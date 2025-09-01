using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks
{
    public class RegolithBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.RegolithBrick>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Regolith>(2)
                .AddTile(TileID.Furnaces)
                .Register();

            CreateRecipe()
                .AddIngredient<RegolithBrickWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}