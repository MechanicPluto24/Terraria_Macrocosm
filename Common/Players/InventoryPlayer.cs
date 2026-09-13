using Macrocosm.Common.Storage;
using Macrocosm.Content.Rockets;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Players;

public class InventoryPlayer : ModPlayer
{
    public override void Load()
    {
        On_Player.QuickStackAllChests += On_Player_QuickStackAllChests;
        On_ItemSlot.SetGlow += On_ItemSlot_SetGlow;
        On_ItemSorting.Sort += On_ItemSorting_Sort;
    }

    public override void Unload()
    {
        On_Player.QuickStackAllChests -= On_Player_QuickStackAllChests;
        On_ItemSlot.SetGlow -= On_ItemSlot_SetGlow;
        On_ItemSorting.Sort -= On_ItemSorting_Sort;
    }

    // Quick store hover icon to custom inventory 
    public override bool HoverSlot(Item[] inventory, int context, int slot)
    {
        if (Inventory.CustomInventoriesActive && ItemSlot.ShiftInUse && context == ItemSlot.Context.InventoryItem)
        {
            Item item = inventory[slot];

            if (Inventory.ActiveInventories.Any(active => active.TryPlacingItem(ref item, InventoryPlacementSource.Player, justCheck: true)))
                Main.cursorOverride = CursorOverrideID.InventoryToChest;

            return true;
        }

        return false;
    }

    // Shift click to custom inventory
    public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
    {
        if (Inventory.CustomInventoriesActive && ItemSlot.ShiftInUse && context == ItemSlot.Context.InventoryItem)
        {
            Item item = inventory[slot];
            foreach (Inventory active in Inventory.ActiveInventories)
                if (active.TryPlacingItem(ref item, InventoryPlacementSource.Player))
                    return true;
        }

        return false;
    }

    private void On_Player_QuickStackAllChests(On_Player.orig_QuickStackAllChests orig, Player player)
    {
        orig(player);

        // TODO (1.4.5): Replace this legacy reach adapter with QuickStacking/NearbyChests.
        // Match vanilla's 600-pixel chest range, Smart Stack, void-bag sourcing, and server-side transfers.
        TileReachCheckSettings settings = new TileReachCheckSettings { OverrideXReach = 39, OverrideYReach = 39 };

        if (player.whoAmI == Main.myPlayer && !player.HasLockedInventory())
        {
            for (int i = 0; i < RocketManager.MaxRockets; i++)
            {
                Rocket rocket = RocketManager.Rockets[i];

                if (rocket.ActiveInCurrentWorld &&
                    rocket.Bounds.InPlayerInteractionRange(settings) &&
                    rocket.State == Rocket.ActionState.Idle &&
                    !player.GetModPlayer<RocketPlayer>().InRocket
                )
                {
                    rocket.Inventory.QuickStack(rocket.Center);
                }
            }

            foreach (var kvp in TileEntity.ByPosition)
            {
                TileEntity entity = kvp.Value;
                Point16 tileCoordinates = kvp.Key;

                if (entity is IInventoryOwner inventoryOwner &&
                     Main.LocalPlayer.IsInTileInteractionRange(tileCoordinates.X, tileCoordinates.Y, settings)
                )
                {
                    foreach (Inventory inventory in inventoryOwner.GetInventories())
                        inventory.QuickStack(inventoryOwner.InventoryPosition);
                }
            }
        }
    }


    // Avoid sorting player's inventory if a custom inventory is displaying
    private void On_ItemSorting_Sort(On_ItemSorting.orig_Sort orig, bool withFeedback, Item[] inv, int[] ignoreSlots)
    {
        // Detect whether the sorting has been run on the player's inventory (which ignores the hotbar, indexes 0-9)
        bool sortRanOnPlayerInventory = ignoreSlots.Distinct().OrderBy(n => n).Take(10).SequenceEqual(Enumerable.Range(0, 10));

        // Don't sort it if a custom inventory is currently active
        if (Inventory.CustomInventoriesActive && sortRanOnPlayerInventory)
            return;

        orig(withFeedback, inv, ignoreSlots);
    }

    // If a custom inventory is currently active, apply the on-sort glow on the custom inventory instead
    private void On_ItemSlot_SetGlow(On_ItemSlot.orig_SetGlow orig, int index, float hue, bool chest)
    {
        if (Inventory.CustomInventoriesActive)
        {
            foreach (Inventory inventory in Inventory.ActiveInventories)
                if (index < inventory.Items.Length)
                    inventory.SetGlow(index, 300, hue);

            return;
        }

        orig(index, hue, chest);
    }

}
