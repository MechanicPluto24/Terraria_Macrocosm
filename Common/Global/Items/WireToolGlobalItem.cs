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
                switch (item.type)
                {
                    case ItemID.WireCutter:
                        if (Main.tile[targetCoords].HasWire())
                            return null;

                        return ConveyorSystem.Remove(targetCoords) ? true : null;

                    case ItemID.Wrench:
                        if (PickPipe(player))
                        {
                            ConveyorSystem.PlacePipe(targetCoords, ConveyorPipeType.RedPipe);
                        }
                        else return null;
                        break;

                    case ItemID.BlueWrench:
                        break;

                    case ItemID.GreenWrench:
                        break;

                    case ItemID.YellowWrench:
                        break;
                }

            }

            return null;
        }

        private bool PickPipe(Player player)
        {
            for (int i = 55; i < 59; i++)
            {
                Item item = player.inventory[i];
                if (item.type == ItemID.Wire)
                    return false;

                if (item.type == ModContent.ItemType<Conveyor>())
                    return true;
            }

            for (int i = 0; i < 55; i++)
            {
                Item item = player.inventory[i];
                if (item.type == ItemID.Wire)
                    return false;

                if (item.type == ModContent.ItemType<Conveyor>())
                    return true;
            }

            return false;
        }
    }
}
