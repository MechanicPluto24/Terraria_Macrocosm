using Macrocosm.Content.Items.Blocks.Terrain;
using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks
{
    public class HaemonovaBrick : ModItem
    {

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.HaemonovaBrick>());
        }

        public override void AddRecipes()
        {
        }
    }
}