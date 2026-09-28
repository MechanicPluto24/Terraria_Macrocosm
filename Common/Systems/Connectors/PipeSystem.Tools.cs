using Macrocosm.Common.Netcode;
using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Connectors;

public partial class PipeSystem
{
    public static void RequestToolOperation(Player player, Point start, Point end, PipeToolState state, bool verticalFirst)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            ExecuteToolOperation(player, start, end, state, verticalFirst);
            return;
        }
        var packet = Macrocosm.Instance.GetPacket();
        packet.Write((byte)MessageType.PipeToolOperation);
        packet.Write((short)start.X); packet.Write((short)start.Y);
        packet.Write((short)end.X); packet.Write((short)end.Y);
        packet.Write(state.Colors); packet.Write((byte)state.Component);
        packet.Write(state.Cutting); packet.Write(verticalFirst);
        packet.Send();
    }

    public static void ReceiveToolOperation(BinaryReader reader, int sender)
    {
        Point start = new(reader.ReadInt16(), reader.ReadInt16());
        Point end = new(reader.ReadInt16(), reader.ReadInt16());
        var state = new PipeToolState { Colors = reader.ReadByte(), Component = (PipeToolComponent)reader.ReadByte(), Cutting = reader.ReadBoolean() };
        bool verticalFirst = reader.ReadBoolean();
        if (Main.netMode == NetmodeID.Server && sender >= 0 && sender < Main.maxPlayers)
            ExecuteToolOperation(Main.player[sender], start, end, state, verticalFirst);
    }

    public static bool InToolReach(Player player, Point p)
        => p.X >= player.position.X / 16f - Player.tileRangeX - player.HeldItem.tileBoost
        && p.X <= (player.position.X + player.width) / 16f + Player.tileRangeX + player.HeldItem.tileBoost - 1
        && p.Y >= player.position.Y / 16f - Player.tileRangeY - player.HeldItem.tileBoost
        && p.Y <= (player.position.Y + player.height) / 16f + Player.tileRangeY + player.HeldItem.tileBoost - 2;

    // New tools use one authoritative mutation path; legacy tools keep their existing behavior.
    public static void ExecuteToolOperation(Player player, Point start, Point end, PipeToolState state, bool verticalFirst)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || !state.IsValid || !player.active || player.dead || player.CCed || player.noItems || player.noBuilding)
            return;
        bool tablet = player.HeldItem.ModItem is PipeGrandDesign;
        if (!tablet && player.HeldItem.ModItem is not MulticoloredPipeWrench) return;
        if (!WorldGen.InWorld(start.X, start.Y, 1) || !WorldGen.InWorld(end.X, end.Y, 1)) return;
        if ((!tablet || state.Component != PipeToolComponent.Pipes) && start != end) return;
        if (!tablet && (state.Component != PipeToolComponent.Pipes || !InToolReach(player, end))) return;
        var toolPlayer = player.GetModPlayer<PipeToolPlayer>();
        if (Main.GameUpdateCount < toolPlayer.NextOperation) return;
        toolPlayer.NextOperation = Main.GameUpdateCount + 10;
        Dictionary<int, int> refunds = new();
        HashSet<int> slots = new();
        bool changed = false;
        foreach (Point p in PipeToolState.Path(start, end, verticalFirst))
        {
            if (!player.CanDoWireStuffHere(p.X, p.Y)) continue;
            ref var data = ref Main.tile[p.X, p.Y].Get<PipeData>();
            ushort before = data.Packed;
            bool exhausted = false;
            if (state.Cutting)
                RemoveSelected(ref data, state, refunds);
            else if (state.Component == PipeToolComponent.Pipes)
            {
                for (int c = 0; c < 4; c++)
                {
                    var color = (PipeType)c;
                    if ((state.Colors & (1 << c)) == 0 || data.HasPipe(color)) continue;
                    if (!ConsumeToolMaterial(player, ModContent.ItemType<Pipe>(), slots)) { exhausted = true; break; }
                    data.SetPipe(color);
                }
            }
            else if (state.Component == PipeToolComponent.Inlet || state.Component == PipeToolComponent.Outlet)
            {
                bool inlet = state.Component == PipeToolComponent.Inlet;
                if (data.AnyPipe && !(inlet ? data.Inlet : data.Outlet) && ConsumeToolMaterial(player, ComponentItem(state.Component), slots))
                {
                    if (data.Inlet || data.Outlet) Refund(refunds, ComponentItem(data.Inlet ? PipeToolComponent.Inlet : PipeToolComponent.Outlet));
                    if (inlet) data.Inlet = true; else data.Outlet = true;
                }
            }
            else
            {
                bool hopper = state.Component == PipeToolComponent.Hopper;
                if (data.Attachment && data.AttachmentIsHopper == hopper)
                    data.AttachmentRotation = (byte)((data.AttachmentRotation + 1) % 4);
                else if (ConsumeToolMaterial(player, ComponentItem(state.Component), slots))
                {
                    if (data.Attachment) Refund(refunds, ComponentItem(data.AttachmentIsHopper ? PipeToolComponent.Hopper : PipeToolComponent.Dropper));
                    data.Attachment = true;
                    data.AttachmentIsHopper = hopper;
                    data.AttachmentRotation = 3;
                }
            }
            if (before != data.Packed)
            {
                changed = true;
                RefreshAttachmentState(new Point16(p.X, p.Y), data);
                if (Main.netMode == NetmodeID.Server) SyncPipe(p.X, p.Y);
            }
            if (exhausted) break;
        }
        foreach (var pair in refunds)
            Item.NewItem(player.GetSource_Misc("PipeTool"), player.Hitbox, pair.Key, pair.Value);
        foreach (int slot in slots)
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, player.whoAmI, slot);
        if (Main.netMode == NetmodeID.Server)
        {
            // Vanilla equipment sync deliberately ignores the owning client.
            var packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.PipeToolInventory);
            packet.Write(changed);
            packet.Write(state.Cutting);
            packet.Write((byte)slots.Count);
            foreach (int slot in slots)
            {
                packet.Write((byte)slot);
                ItemIO.Send(player.inventory[slot], packet, true, true);
            }
            packet.Send(player.whoAmI);
        }
        if (changed)
        {
            buildTimer = (int)global::Macrocosm.Common.Config.ServerConfig.Instance.CircuitSolveUpdateRate;
            if (Main.netMode != NetmodeID.Server)
                SoundEngine.PlaySound(state.Cutting ? SoundID.Dig : SoundID.Mech, player.Center);
        }
    }

    public static void ReceiveToolInventory(BinaryReader reader)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient) return;
        bool changed = reader.ReadBoolean();
        bool cutting = reader.ReadBoolean();
        int count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            int slot = reader.ReadByte();
            Item item = ItemIO.Receive(reader, true, true);
            if (slot < 59) Main.LocalPlayer.inventory[slot] = item;
        }
        if (count > 0) Recipe.FindRecipes();
        if (changed) SoundEngine.PlaySound(cutting ? SoundID.Dig : SoundID.Mech, Main.LocalPlayer.Center);
    }

    public static void RemoveSelected(ref PipeData data, PipeToolState state, Dictionary<int, int> refunds)
    {
        if (state.Component == PipeToolComponent.Pipes)
        {
            int endpoint = data.Inlet ? ComponentItem(PipeToolComponent.Inlet) : data.Outlet ? ComponentItem(PipeToolComponent.Outlet) : 0;
            for (int c = 0; c < 4; c++)
                if ((state.Colors & (1 << c)) != 0 && data.HasPipe((PipeType)c))
                { data.ClearPipe((PipeType)c); Refund(refunds, ModContent.ItemType<Pipe>()); }
            if (!data.AnyPipe && endpoint != 0) Refund(refunds, endpoint);
        }
        else if (state.Component == PipeToolComponent.Inlet && data.Inlet)
        { data.Inlet = false; Refund(refunds, ComponentItem(state.Component)); }
        else if (state.Component == PipeToolComponent.Outlet && data.Outlet)
        { data.Outlet = false; Refund(refunds, ComponentItem(state.Component)); }
        else if (data.Attachment && (state.Component == PipeToolComponent.Hopper && data.AttachmentIsHopper || state.Component == PipeToolComponent.Dropper && !data.AttachmentIsHopper))
        { data.Attachment = false; Refund(refunds, ComponentItem(state.Component)); }
    }

    private static void Refund(Dictionary<int, int> refunds, int type)
        => refunds[type] = refunds.GetValueOrDefault(type) + 1;

    private static int ComponentItem(PipeToolComponent component) => component switch
    {
        PipeToolComponent.Inlet => ModContent.ItemType<PipeInlet>(),
        PipeToolComponent.Outlet => ModContent.ItemType<PipeOutlet>(),
        PipeToolComponent.Hopper => ModContent.ItemType<Hopper>(),
        PipeToolComponent.Dropper => ModContent.ItemType<Dropper>(),
        _ => ModContent.ItemType<Pipe>()
    };

    private static bool ConsumeToolMaterial(Player player, int type, HashSet<int> changedSlots)
    {
        for (int n = 0; n < 59; n++)
        {
            int slot = n < 4 ? 55 + n : n - 4;
            Item item = player.inventory[slot];
            if (item.type != type || item.stack <= 0) continue;
            if (ItemLoader.ConsumeItem(item, player))
            {
                if (--item.stack == 0) item.TurnToAir();
                changedSlots.Add(slot);
            }
            return true;
        }
        return false;
    }
}
