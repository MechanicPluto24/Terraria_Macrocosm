using Macrocosm.Content.Items.Food;
using Macrocosm.Content.Items.Furniture.Cheese;
using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks
{
    public class CheeseBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.CheeseBlock>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient<Cheese>()
                .Register();

            CreateRecipe()
                .AddIngredient<CheesePlatform>(2)
                .Register();

            CreateRecipe()
               .AddIngredient<CheeseWall>(4)
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
}