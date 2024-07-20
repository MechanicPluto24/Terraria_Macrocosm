using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    public class IndustrialNeonTube : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialNeonTube>());
            Item.width = 30;
            Item.height = 8;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IndustrialPlating>(4)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}
