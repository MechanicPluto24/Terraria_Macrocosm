using Macrocosm.Common.Config;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

    private static Asset<Texture2D> dropperTexture;
    private static readonly Dictionary<Point16, DropperState> dropperStates = new();

    private enum DropperOrientation : byte
    {
        Down,
        Left,
        Up,
        Right
    }

    private struct DropperState
    {
        public int Cooldown;

        public DropperState(int cooldown)
        {
            Cooldown = cooldown;
        }
    }

    private static void ClearDroppers() => dropperStates.Clear();

    public static bool PlaceDropper(Point targetCoords, bool sync = true) => PlaceDropper(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceDropper(Point16 targetCoords, bool sync = true) => PlaceDropper(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceDropper(int x, int y, bool sync = true)
    {
        if (!WorldGen.InWorld(x, y))
            return false;

        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (data.Dropper)
            return false;

        data.Dropper = true;
        data.DropperRotation = 0;

        Point16 pos = new(x, y);
        dropperStates[pos] = new DropperState(DropperCooldownTicks);

        DustEffects(x, y);

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y, dustEffects: true);

        buildTimer = (int)ServerConfig.Instance.CircuitSolveUpdateRate;

        return true;
    }

    private static bool TryRemoveDropper(int x, int y, bool sync)
    {
        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (!data.Dropper)
            return false;

        data.Dropper = false;
        data.DropperRotation = 0;

        Point16 pos = new(x, y);
        dropperStates.Remove(pos);

        DustEffects(x, y);

        if (Main.netMode != NetmodeID.MultiplayerClient)
            Item.NewItem(new EntitySource_TileBreak(x, x, "Conveyor"), new Vector2(x * 16 + 8, y * 16 + 8), ModContent.ItemType<Dropper>());

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y, dustEffects: true);

        return true;
    }

    public static bool TryRotateDropper(int x, int y, bool sync = true)
    {
        if (!ShouldDraw)
            return false;

        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (!data.Dropper)
            return false;

        Point16 pos = new(x, y);
        if (!ShouldHighlightDropper(pos))
            return false;

        DropperOrientation next = NextOrientation(GetDropperOrientation(pos));
        ApplyDropperOrientation(pos, next);

        SoundEngine.PlaySound(SoundID.MenuTick with { Volume = 0.6f }, pos.ToWorldCoordinates(8f, 8f));

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y);

        return true;
    }

    private static void UpdateDroppers()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || dropperStates.Count == 0)
            return;

        foreach (var kvp in dropperStates.ToArray())
        {
            Point16 pos = kvp.Key;
            ref var data = ref Main.tile[pos.X, pos.Y].Get<ConveyorData>();
            if (!data.Dropper)
            {
                dropperStates.Remove(pos);
                continue;
            }

            DropperState state = kvp.Value;
            if (state.Cooldown > 0)
            {
                state.Cooldown--;
                dropperStates[pos] = state;
                continue;
            }

            if (!TryGetConveyorNode(pos, out var node))
                continue;

            if (!TryExtractItem(node, out Item droppedItem))
                continue;

            SpawnDroppedItem(pos, droppedItem, GetDropperOrientation(pos));
            state.Cooldown = DropperCooldownTicks;
            dropperStates[pos] = state;
        }
    }

    private static void DrawDropper(SpriteBatch spriteBatch, Point16 pos, Vector2 offset)
    {
        if (dropperTexture?.Value is null)
            return;

        Vector2 basePosition = new Vector2(pos.X * 16 - (int)Main.screenPosition.X, pos.Y * 16 - (int)Main.screenPosition.Y) + offset + new Vector2(8f);

        DropperOrientation orientation = GetDropperOrientation(pos);
        float rotation = MathHelper.PiOver2 * (int)orientation;

        bool highlight = ShouldHighlightDropper(pos);
        Color drawColor = highlight ? Color.Gold : Lighting.GetColor(pos.X, pos.Y);
        float scale = highlight ? 1.15f : 1f;

        spriteBatch.Draw(dropperTexture.Value, basePosition, null, drawColor, rotation, new Vector2(8f, 8f), scale, SpriteEffects.None, 0f);
    }

    private static bool ShouldHighlightDropper(Point16 pos)
    {
        if (!ShouldDraw || Main.LocalPlayer is null)
            return false;

        Player player = Main.LocalPlayer;
        if (player.dead)
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

    private static DropperOrientation GetDropperOrientation(Point16 pos)
    {
        ref var data = ref Main.tile[pos.X, pos.Y].Get<ConveyorData>();
        return (DropperOrientation)(data.DropperRotation & 0b11);
    }

    private static DropperOrientation NextOrientation(DropperOrientation current) => (DropperOrientation)(((int)current + 1) % 4);

    private static void ApplyDropperOrientation(Point16 pos, DropperOrientation orientation)
    {
        ref var data = ref Main.tile[pos.X, pos.Y].Get<ConveyorData>();
        data.DropperRotation = (byte)orientation;
        RefreshDropperState(pos, data);
    }

    private static void SpawnDroppedItem(Point16 pos, Item item, DropperOrientation orientation)
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
                DropperOrientation.Left => new Vector2(-4f, 0f),
                DropperOrientation.Up => new Vector2(0f, -4f),
                DropperOrientation.Right => new Vector2(4f, 0f),
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

    private static void RefreshDropperState(Point16 pos, ConveyorData data)
    {
        if (data.Dropper)
        {
            if (!dropperStates.ContainsKey(pos))
                dropperStates[pos] = new DropperState(DropperCooldownTicks);
        }
        else
        {
            dropperStates.Remove(pos);
        }
    }

    private static void RebuildDropperStateCache()
    {
        dropperStates.Clear();

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                ref var data = ref Main.tile[x, y].Get<ConveyorData>();
                if (data.Dropper && data.AnyPipe)
                    dropperStates[new Point16(x, y)] = new DropperState(DropperCooldownTicks);
            }
        }
    }
}
