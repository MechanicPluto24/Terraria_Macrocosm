using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.Regolith
{
    public class RegolithWorkbench : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Regolith.RegolithWorkbench>());
            Item.width = 32;
            Item.height = 18;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RegolithBrick>(10)
                .Register();
        }
    }
}
