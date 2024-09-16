using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    [LegacyName("MoonBaseWorkbench")]
    public class IndustrialWorkbench : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialWorkbench>());
            Item.width = 32;
            Item.height = 16;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IndustrialPlating>(6)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}