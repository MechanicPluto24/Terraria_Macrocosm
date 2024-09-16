using Macrocosm.Content.Items.Food;
using Terraria;
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
        }
    }
}