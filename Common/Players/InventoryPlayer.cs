using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Players
{
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
            if (Inventory.CustomInventoryActive && ItemSlot.ShiftInUse && context == ItemSlot.Context.InventoryItem)
            {
                Item item = inventory[slot];

                if (Inventory.ActiveInventory.TryPlacingItem(ref item, justCheck: true))
                    Main.cursorOverride = CursorOverrideID.InventoryToChest;

                return true;
            }

            return false;
        }

        // Shift click to custom inventory
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (Inventory.CustomInventoryActive && ItemSlot.ShiftInUse && context == ItemSlot.Context.InventoryItem)
            {
                Item item = inventory[slot];
                if (Inventory.ActiveInventory.TryPlacingItem(ref item))
                    return true;
            }

            return false;
        }

        private void On_Player_QuickStackAllChests(On_Player.orig_QuickStackAllChests orig, Player player)
        {
            orig(player);

            TileReachCheckSettings settings = TileReachCheckSettings.QuickStackToNearbyChests;

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
                        ContainerTransferContext transferContext = new(rocket.Center);
                        rocket.Inventory.QuickStack(transferContext);
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
                        ContainerTransferContext transferContext = new(inventoryOwner.InventoryPosition);
                        inventoryOwner.Inventory.QuickStack(transferContext);
                    }
                }
            }
        }


        // Avoid sorting player's inventory if a custom inventory is displaying
        private void On_ItemSorting_Sort(On_ItemSorting.orig_Sort orig, Item[] inv, int[] ignoreSlots)
        {
            // Detect whether the sorting has been run on the player's inventory (which ignores the hotbar, indexes 0-9)
            bool sortRanOnPlayerInventory = ignoreSlots.Distinct().OrderBy(n => n).Take(10).SequenceEqual(Enumerable.Range(0, 10));

            // Don't sort it if a custom inventory is currently active
            if (Inventory.CustomInventoryActive && sortRanOnPlayerInventory)
                return;

            orig(inv, ignoreSlots);
        }

        // If a custom inventory is currently active, apply the on-sort glow on the custom inventory instead
        private void On_ItemSlot_SetGlow(On_ItemSlot.orig_SetGlow orig, int index, float hue, bool chest)
        {
            if (Inventory.CustomInventoryActive)
            {
                if (index < Inventory.ActiveInventory.Items.Length)
                    Inventory.ActiveInventory.SetGlow(index, 300, hue);

                return;
            }

            orig(index, hue, chest);
        }

    }
}
