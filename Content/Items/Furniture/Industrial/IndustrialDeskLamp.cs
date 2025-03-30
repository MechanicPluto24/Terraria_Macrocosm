using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    public class IndustrialDeskLamp : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialDeskLamp>());
            Item.width = 32;
            Item.height = 30;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IndustrialPlating>(5)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}
