using Macrocosm.Content.Items.Blocks;
using Terraria;
using Macrocosm.Content.Items.Bars;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture
{
    public class Flag : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Flag>(), 0);
            Item.width = 16;
            Item.height = 32;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<AluminumBar>(3)
            .AddIngredient(ItemID.Silk, 10)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}