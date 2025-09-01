using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Cheese
{
    public class CheeseBathtub : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Cheese.CheeseBathtub>());
            Item.width = 38;
            Item.height = 24;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CheeseBlock>(14)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
