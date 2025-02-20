using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Liquids;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers
{
    public class RocketFuelBucket : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.LiquidContainerData[Type] = new LiquidContainerData(ModLiquidLib.ModLiquidLib.LiquidType<RocketFuel>(), 50, ItemID.EmptyBucket);
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.stack = 9999;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Utility.PlaceLiquid<Oil>(Player.tileTargetX, Player.tileTargetY);
                Utility.TransformItemAndPutInInventory(player, Item, ItemID.EmptyBucket);
            }
            return null;
        }
    }
}
