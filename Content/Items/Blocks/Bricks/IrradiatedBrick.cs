using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks
{
    public class IrradiatedBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.IrradiatedBrick>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IrradiatedRock>(2)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<IrradiatedBrickWall>(4)
                .Register();
        }
    }
}