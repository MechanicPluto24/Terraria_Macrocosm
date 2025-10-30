using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Connectors;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Connectors;

public partial class ConveyorSystem
{
    private static int DropperCooldownTicks => 90;
    private static int DropperGrabDelay => 200;
    private static int HopperCooldownTicks => 15;

    private static Asset<Texture2D> attachmentTexture;

    private static readonly Dictionary<Point16, AttachmentState> attachmentStates = new();

    private enum AttachmentOrientation : byte
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3,
    }

    private struct AttachmentState
    {
        public int Cooldown;

        public AttachmentState(int cooldown)
        {
            Cooldown = cooldown;
        }
    }

    private static void ClearAttachments() => attachmentStates.Clear();
    private static void ClearDroppers() => ClearAttachments();
    private static void ClearHoppers() => ClearAttachments();

    public static bool PlaceDropper(Point targetCoords, bool sync = true) => PlaceDropper(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceDropper(Point16 targetCoords, bool sync = true) => PlaceDropper(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceDropper(int x, int y, bool sync = true)
    {
        if (!WorldGen.InWorld(x, y))
            return false;

        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (data.Attachment)
            return false; // mutually exclusive; already has an attachment

        data.Dropper = true; // sets Attachment and type
        data.AttachmentRotation = (byte)AttachmentOrientation.Down;

        Point16 pos = new(x, y);
        attachmentStates[pos] = new AttachmentState(DropperCooldownTicks);

        DustEffects(x, y);

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y, dustEffects: true);

        buildTimer = (int)ServerConfig.Instance.CircuitSolveUpdateRate;

        return true;
    }

    public static bool PlaceHopper(Point targetCoords, bool sync = true) => PlaceHopper(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceHopper(Point16 targetCoords, bool sync = true) => PlaceHopper(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceHopper(int x, int y, bool sync = true)
    {
        if (!WorldGen.InWorld(x, y))
            return false;

        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (data.Attachment)
            return false; // mutually exclusive; already has an attachment

        data.Hopper = true; // sets Attachment and type
        data.AttachmentRotation = (byte)AttachmentOrientation.Down;

        Point16 pos = new(x, y);
        attachmentStates[pos] = new AttachmentState(HopperCooldownTicks);

        DustEffects(x, y);

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y, dustEffects: true);

        buildTimer = (int)ServerConfig.Instance.CircuitSolveUpdateRate;

        return true;
    }

    private static bool TryRemoveDropper(int x, int y, bool sync)
        => TryRemoveAttachment(x, y, expectedHopper: false, sync: sync);

    private static bool TryRemoveHopper(int x, int y, bool sync)
        => TryRemoveAttachment(x, y, expectedHopper: true, sync: sync);

    private static bool TryRemoveAttachment(int x, int y, bool expectedHopper, bool sync)
    {
        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (!data.Attachment || data.AttachmentIsHopper != expectedHopper)
            return false;

        data.Attachment = false;
        data.AttachmentRotation = 0;

        Point16 pos = new(x, y);
        attachmentStates.Remove(pos);

        DustEffects(x, y);

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            int itemType = expectedHopper ? ModContent.ItemType<Hopper>() : ModContent.ItemType<Dropper>();
            Item.NewItem(new EntitySource_TileBreak(x, x, "Conveyor"), new Vector2(x * 16 + 8, y * 16 + 8), itemType);
        }

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y, dustEffects: true);

        return true;
    }

    public static bool TryRotateDropper(int x, int y, bool sync = true)
        => TryRotateAttachment(x, y, expectedHopper: false, sync);

    public static bool TryRotateHopper(int x, int y, bool sync = true)
        => TryRotateAttachment(x, y, expectedHopper: true, sync);

    private static bool TryRotateAttachment(int x, int y, bool expectedHopper, bool sync)
    {
        if (!ShouldDraw)
            return false;

        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (!data.Attachment || data.AttachmentIsHopper != expectedHopper)
            return false;

        Point16 pos = new(x, y);
        if (!ShouldHighlightAttachment(pos))
            return false;

        AttachmentOrientation next = NextOrientation(GetAttachmentOrientation(pos));
        ApplyAttachmentOrientation(pos, next);

        SoundEngine.PlaySound(SoundID.MenuTick with { Volume = 0.6f }, pos.ToWorldCoordinates(8f, 8f));

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y);

        return true;
    }

    private static void UpdateAttachments()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || attachmentStates.Count == 0)
            return;

        foreach (var kvp in attachmentStates.ToArray())
        {
            Point16 pos = kvp.Key;
            ref var data = ref Main.tile[pos.X, pos.Y].Get<ConveyorData>();
            if (!data.Attachment)
            {
                attachmentStates.Remove(pos);
                continue;
            }

            AttachmentState state = kvp.Value;
            if (state.Cooldown > 0)
            {
                state.Cooldown--;
                attachmentStates[pos] = state;
                continue;
            }

            bool isHopper = data.AttachmentIsHopper;

            // If attached to a provider tile, act on that container
            if (TryGetConveyorNode(pos, out var node))
            {
                if (isHopper)
                {
                    // Pick up from world into the container
                    if (TryPickupWorldItem(pos, out int itemIndex))
                    {
                        Item picked = Main.item[itemIndex];
                        if (TryDepositItem(node, ref picked))
                        {
                            SpawnPickupParticle(pos, itemIndex);
                            // consumed one unit by deposit logic; reduce world item
                            ConsumeWorldItem(itemIndex);
                            state.Cooldown = HopperCooldownTicks;
                            attachmentStates[pos] = state;
                        }
                    }
                }
                else
                {
                    // Dropper: extract from container and drop into world
                    if (TryExtractItem(node, out Item droppedItem))
                    {
                        SpawnDroppedItem(pos, droppedItem, GetAttachmentOrientation(pos));
                        state.Cooldown = DropperCooldownTicks;
                        attachmentStates[pos] = state;
                    }
                }
                continue;
            }

            // Not on a provider tile. If on pipes, act as virtual outlet/inlet in the circuit.
            if (data.AnyPipe)
            {
                // For each pipe type present at this tile
                for (ConveyorPipeType type = 0; type < ConveyorPipeType.Count; type++)
                {
                    if (!data.HasPipe(type))
                        continue;

                    var connectedNodes = FindConnectedNodesFrom(pos, type);
                    if (connectedNodes.Count == 0)
                        continue;

                    if (isHopper)
                    {
                        // Hopper as outlet: pick up from world and distribute to inlets
                        if (TryPickupWorldItem(pos, out int itemIndex))
                        {
                            Item picked = Main.item[itemIndex];
                            if (TryDistributeToInlets(connectedNodes, ref picked))
                            {
                                SpawnPickupParticle(pos, itemIndex);
                                ConsumeWorldItem(itemIndex);
                                state.Cooldown = HopperCooldownTicks;
                                attachmentStates[pos] = state;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Dropper as inlet: pull from outlets, then drop
                        if (TryPullFromOutlets(connectedNodes, out Item pulled))
                        {
                            SpawnDroppedItem(pos, pulled, GetAttachmentOrientation(pos));
                            state.Cooldown = DropperCooldownTicks;
                            attachmentStates[pos] = state;
                            break;
                        }
                    }
                }
            }
        }
    }

    private static void DrawAttachment(SpriteBatch spriteBatch, Point16 pos, Vector2 offset)
    {
        ref var data = ref Main.tile[pos.X, pos.Y].Get<ConveyorData>();
        if (!data.Attachment)
            return;

        if (attachmentTexture?.Value is null)
            return;

        Vector2 drawPosition = new Vector2(pos.X * 16 - (int)Main.screenPosition.X, pos.Y * 16 - (int)Main.screenPosition.Y) + offset;
        AttachmentOrientation orientation = GetAttachmentOrientation(pos);
        Rectangle frame = GetAttachmentFrame(data.AttachmentIsHopper, orientation);

        bool highlight = ShouldHighlightAttachment(pos);
        Color drawColor = highlight ? Color.Gold.WithAlpha(220) : Lighting.GetColor(pos.X, pos.Y);

        spriteBatch.Draw(attachmentTexture.Value, drawPosition, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    private static Rectangle GetAttachmentFrame(bool isHopper, AttachmentOrientation orientation)
    {
        int frameY = isHopper ? 1 : 0;
        int frameX = (int)orientation;
        return new Rectangle(frameX * 18, frameY * 18, 16, 16);
    }

    private static bool ShouldHighlightAttachment(Point16 pos)
    {
        if (!ShouldDraw || Main.LocalPlayer is null)
            return false;

        Player player = Main.LocalPlayer;
        if (player.dead)
            return false;

        // Require holding a hammer to interact/rotate
        if (player.HeldItem is null || player.HeldItem.hammer <= 0)
            return false;

        Point target = player.TargetCoords();
        if (target.X != pos.X || target.Y != pos.Y)
            return false;

        if (!player.CanDoWireStuffHere(pos.X, pos.Y))
            return false;

        if (!player.IsInTileInteractionRange(pos.X, pos.Y, TileReachCheckSettings.Simple))
            return false;

        return true;
    }

    private static AttachmentOrientation GetAttachmentOrientation(Point16 pos)
    {
        ref var data = ref Main.tile[pos.X, pos.Y].Get<ConveyorData>();
        return (AttachmentOrientation)(data.AttachmentRotation & 0b11);
    }

    private static AttachmentOrientation NextOrientation(AttachmentOrientation current) => (AttachmentOrientation)(((int)current + 1) % 4);

    private static void ApplyAttachmentOrientation(Point16 pos, AttachmentOrientation orientation)
    {
        ref var data = ref Main.tile[pos.X, pos.Y].Get<ConveyorData>();
        data.AttachmentRotation = (byte)orientation;
        RefreshAttachmentState(pos, data);
    }

    private static void SpawnDroppedItem(Point16 pos, Item item, AttachmentOrientation orientation)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || item is null)
            return;

        Item drop = item.Clone();
        drop.stack = 1;

        Vector2 spawnPosition = pos.ToWorldCoordinates(8f, 8f);
        int index = Item.NewItem(new EntitySource_WorldEvent("Macrocosm:ConveyorDropper"), spawnPosition, drop);
        if (index >= 0 && index < Main.maxItems)
        {
            Main.item[index].velocity = orientation switch
            {
                AttachmentOrientation.Left => new Vector2(-4f, 0f),
                AttachmentOrientation.Up => new Vector2(0f, -4f),
                AttachmentOrientation.Right => new Vector2(4f, 0f),
                _ => Vector2.Zero
            };
            Main.item[index].noGrabDelay = DropperGrabDelay;
        }
    }

    private static bool TryExtractItem(ConveyorNode node, out Item result)
    {
        result = null;

        if (node.Entity is Chest chest)
            return TryExtractFromChest(chest, out result);

        if (node.Entity is IInventoryOwner owner)
            return TryExtractFromInventory(owner, out result);

        return false;
    }

    private static bool TryExtractFromChest(Chest chest, out Item result)
    {
        result = null;
        if (chest is null)
            return false;

        for (int slot = 0; slot < chest.item.Length; slot++)
        {
            Item chestItem = chest.item[slot];
            if (chestItem is null || chestItem.IsAir)
                continue;

            result = chestItem.Clone();
            result.stack = 1;

            chestItem.stack--;
            if (chestItem.stack <= 0)
                chestItem.TurnToAir();

            if (Main.netMode == NetmodeID.Server)
            {
                int chestIndex = Chest.FindChest(chest.x, chest.y);
                if (chestIndex >= 0)
                    NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, chestIndex, slot);
            }

            return true;
        }

        return false;
    }

    private static bool TryExtractFromInventory(IInventoryOwner owner, out Item result)
    {
        result = null;
        Inventory inventory = owner?.Inventory;
        if (inventory is null)
            return false;

        for (int slot = 0; slot < inventory.Size; slot++)
        {
            Item item = inventory[slot];
            if (item is null || item.IsAir)
                continue;

            result = item.Clone();
            result.stack = 1;

            item.stack--;
            if (item.stack <= 0)
                item.TurnToAir();

            inventory.SyncItem(slot);
            return true;
        }

        return false;
    }

    private static bool TryDepositItem(ConveyorNode node, ref Item worldItem)
    {
        if (worldItem is null || worldItem.IsAir)
            return false;

        if (node.Entity is Chest chest)
            return TryDepositIntoChest(chest, ref worldItem);

        if (node.Entity is IInventoryOwner owner)
            return TryDepositIntoInventory(owner, ref worldItem);

        return false;
    }

    private static bool TryDepositIntoChest(Chest chest, ref Item worldItem)
    {
        if (chest is null || worldItem is null || worldItem.IsAir)
            return false;

        int chestIndex = Chest.FindChest(chest.x, chest.y);

        // Try stack first
        for (int slot = 0; slot < chest.item.Length; slot++)
        {
            Item slotItem = chest.item[slot];
            if (slotItem is null || slotItem.IsAir)
                continue;

            if (slotItem.type != worldItem.type)
                continue;

            if (!ItemLoader.CanStack(slotItem, worldItem))
                continue;

            if (slotItem.stack >= slotItem.maxStack)
                continue;

            slotItem.stack += 1;
            if (Main.netMode == NetmodeID.Server && chestIndex >= 0)
                NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, chestIndex, slot);
            return true;
        }

        // Try empty slot
        for (int slot = 0; slot < chest.item.Length; slot++)
        {
            Item slotItem = chest.item[slot];
            if (slotItem is null || slotItem.IsAir)
            {
                chest.item[slot] = worldItem.Clone();
                chest.item[slot].stack = 1;
                if (Main.netMode == NetmodeID.Server && chestIndex >= 0)
                    NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, chestIndex, slot);
                return true;
            }
        }

        return false;
    }

    private static bool TryDepositIntoInventory(IInventoryOwner owner, ref Item worldItem)
    {
        Inventory inventory = owner?.Inventory;
        if (inventory is null || worldItem is null || worldItem.IsAir)
            return false;

        // Try stack first
        for (int slot = 0; slot < inventory.Size; slot++)
        {
            Item invItem = inventory[slot];
            if (invItem is null || invItem.IsAir)
                continue;

            if (invItem.type != worldItem.type)
                continue;

            if (!ItemLoader.CanStack(invItem, worldItem))
                continue;

            if (invItem.stack >= invItem.maxStack)
                continue;

            invItem.stack += 1;
            inventory.SyncItem(slot);
            return true;
        }

        // Try empty slot
        for (int slot = 0; slot < inventory.Size; slot++)
        {
            Item invItem = inventory[slot];
            if (invItem is null || invItem.IsAir)
            {
                inventory[slot] = worldItem.Clone();
                inventory[slot].stack = 1;
                inventory.SyncItem(slot);
                return true;
            }
        }

        return false;
    }

    private static bool TryPickupWorldItem(Point16 pos, out int itemIndex)
    {
        itemIndex = -1;

        AttachmentOrientation orientation = GetAttachmentOrientation(pos);
        int left = 1, right = 1, up = 1, down = 1;
        switch (orientation)
        {
            case AttachmentOrientation.Left: left = 3; break;
            case AttachmentOrientation.Right: right = 3; break;
            case AttachmentOrientation.Up: up = 3; break;
            case AttachmentOrientation.Down: down = 3; break;
        }

        Rectangle area = new((pos.X - left) * 16, (pos.Y - up) * 16, 16 * (1 + left + right), 16 * (1 + up + down));
        for (int i = 0; i < Main.maxItems; i++)
        {
            Item it = Main.item[i];
            if (!it.active || it.IsAir)
                continue;

            if (it.getRect().Intersects(area))
            {
                itemIndex = i;
                return true;
            }
        }
        return false;
    }

    private static void SpawnPickupParticle(Point16 pos, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= Main.maxItems)
            return;

        Item it = Main.item[itemIndex];
        if (!it.active || it.IsAir)
            return;

        Vector2 start = it.Center;
        Vector2 end = pos.ToWorldCoordinates(8f, 8f);

        Particle.Create<ItemTransferParticle>((p) =>
        {
            p.StartPosition = start;
            p.EndPosition = end;
            p.ItemType = it.type;
            p.TimeToLive = 45;
            p.DisableFadeIn = true;
        });
    }

    private static void ConsumeWorldItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= Main.maxItems)
            return;

        ref Item worldItem = ref Main.item[itemIndex];
        worldItem.stack--;
        if (worldItem.stack <= 0)
            worldItem.TurnToAir();

        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex);
    }

    private static HashSet<ConveyorNode> FindConnectedNodesFrom(Point16 start, ConveyorPipeType type)
    {
        var search = new ConnectionSearch<ConveyorNode>(
            connectionCheck: p => Main.tile[p.X, p.Y].Get<ConveyorData>().HasPipe(type),
            retrieveNode: p => nodeLookup.TryGetValue(p, out var foundNode) && foundNode.Type == type ? foundNode : null
        );
        return search.FindConnectedNodes(new[] { start });
    }

    private static bool TryDistributeToInlets(HashSet<ConveyorNode> nodes, ref Item worldItem)
    {
        var inlets = nodes.Where(n => n.Inlet).ToList();
        foreach (var inlet in inlets)
        {
            if (TryDepositItem(inlet, ref worldItem))
                return true;
        }
        return false;
    }

    private static bool TryPullFromOutlets(HashSet<ConveyorNode> nodes, out Item pulled)
    {
        pulled = null;
        var outlets = nodes.Where(n => n.Outlet).ToList();
        foreach (var outlet in outlets)
        {
            if (TryExtractItem(outlet, out pulled))
                return true;
        }
        return false;
    }

    private static void RefreshAttachmentState(Point16 pos, ConveyorData data)
    {
        if (data.Attachment)
        {
            if (!attachmentStates.ContainsKey(pos))
            {
                int cd = data.AttachmentIsHopper ? HopperCooldownTicks : DropperCooldownTicks;
                attachmentStates[pos] = new AttachmentState(cd);
            }
        }
        else
        {
            attachmentStates.Remove(pos);
        }
    }

    private static void RebuildAttachmentStateCache()
    {
        attachmentStates.Clear();

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                ref var data = ref Main.tile[x, y].Get<ConveyorData>();
                if (data.Attachment)
                    attachmentStates[new Point16(x, y)] = new AttachmentState(data.AttachmentIsHopper ? HopperCooldownTicks : DropperCooldownTicks);
            }
        }
    }
}
