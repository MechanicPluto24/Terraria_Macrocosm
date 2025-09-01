using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Cheese
{
    public class CheeseChair : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Cheese.CheeseChair>());
            Item.width = 26;
            Item.height = 32;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CheeseBlock>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
