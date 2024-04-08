using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Blocks
{
    public class LexanGlass : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.LexanGlass>());
        }

        public override void AddRecipes()
        {

        }
    }
}