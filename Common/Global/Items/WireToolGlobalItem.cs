using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items
{
    public class WireCutterGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type
            is ItemID.WireCutter
            or ItemID.Wrench
            or ItemID.BlueWrench
            or ItemID.GreenWrench
            or ItemID.YellowWrench;
        }

        public override bool? UseItem(Item item, Player player)
        {
            if(player.whoAmI == Main.myPlayer)
            {
                Point targetCoords = player.TargetCoords();
                if(item.type == ItemID.WireCutter)
                {
                    // Default WireCutter logic if wires are present
                    if (Main.tile[targetCoords].HasWire())
                        return null;

                    // Remove pipe or run default WireCutter logic
                    return ConveyorSystem.Remove(targetCoords);
                }
                else if (TryGetPipeTypeFromWrench(item.type, out ConveyorPipeType pipeType))
                {
                    // Pipe over wires in ammo slots/inventory
                    if (PickPipe(player, skipWires: Main.tile[targetCoords].HasWire()))
                        return ConveyorSystem.PlacePipe(targetCoords, pipeType);

                    return null; // Default wrench if no pipe
                }
            }

            return null;
        }

        /// <summary> Used to pick pipes over wires, in ammo priority </summary>
        private static bool PickPipe(Player player, bool skipWires)
        {
            // Ammo slots first, top to bottom
            for (int i = 55; i < 59; i++)
            {
                Item item = player.inventory[i];
                if (item.type == ItemID.Wire && !skipWires)
                    return false;

                if (item.type == ModContent.ItemType<Conveyor>())
                    return true;
            }

            // Main inventory slots, from top-left
            for (int i = 0; i < 55; i++)
            {
                Item item = player.inventory[i];
                if (item.type == ItemID.Wire && !skipWires)
                    return false;

                if (item.type == ModContent.ItemType<Conveyor>())
                    return true;
            }

            return false;
        }

        /// <summary> Wrench Item ID to <see cref="ConveyorPipeType"/></summary>
        private static bool TryGetPipeTypeFromWrench(int itemType, out ConveyorPipeType pipeType)
        {
            switch (itemType)
            {
                case ItemID.Wrench:
                    pipeType = ConveyorPipeType.RedPipe;
                    return true;

                case ItemID.BlueWrench:
                    pipeType = ConveyorPipeType.BluePipe;
                    return true;

                case ItemID.GreenWrench:
                    pipeType = ConveyorPipeType.GreenPipe;
                    return true;

                case ItemID.YellowWrench:
                    pipeType = ConveyorPipeType.YellowPipe;
                    return true;

                default:
                    pipeType = 0;
                    return false;
            }
        }
    }
}
