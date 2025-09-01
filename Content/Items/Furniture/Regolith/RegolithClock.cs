using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Regolith
{
    public class RegolithClock : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Regolith.RegolithClock>());
            Item.width = 18;
            Item.height = 40;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RegolithBrick>(16)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
