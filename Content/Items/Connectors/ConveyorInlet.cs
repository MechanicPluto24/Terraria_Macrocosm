using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Connectors
{
    public class ConveyorInlet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(copper: 10);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.mech = true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Point targetCoords = player.TargetCoords();
                ConnectorData data = ConnectorSystem.Map[player.TargetCoords()];

                if (!data.AnyConveyor)
                {
                    ConnectorSystem.PlaceConnector(targetCoords, ConnectorType.ConveyorInlet);
                    return true;
                }

                return false;
            }

            return null;
        }
    }
}