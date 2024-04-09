using Macrocosm.Content.Items.Materials.Refined;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Tech
{
    public class PowerJunction : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Tech.PowerJunction>());
            Item.width = 32;
            Item.height = 32;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Battery>(4)
                .AddIngredient<PrintedCircuitBoard>(2)
                .AddIngredient<Plastic>(5)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}