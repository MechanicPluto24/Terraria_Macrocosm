using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    [LegacyName("MoonBaseBulkhead")]
    public class IndustrialBulkhead : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<IndustrialBulkheadClosed>());
            Item.width = 12;
            Item.height = 42;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IndustrialPlating>(10)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}