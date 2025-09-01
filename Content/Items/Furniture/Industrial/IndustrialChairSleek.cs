using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    public class IndustrialChairSleek : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialChair>(), 2);
            Item.width = 16;
            Item.height = 32;
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