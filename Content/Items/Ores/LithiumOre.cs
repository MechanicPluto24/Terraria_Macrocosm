using Macrocosm.Common.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Ores
{
    public class LithiumOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ShimmerSystem.RegisterOverride(ItemID.Hellstone, Type);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 750;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Ores.LithiumOre>();
            Item.placeStyle = 0;
            Item.rare = ItemRarityID.White;


            // Set other Item.X values here
        }

        public override void AddRecipes()
        {

        }
    }
}