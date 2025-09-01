using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class PaleBluePixel : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.PaleBluePixel>());
            Item.width = 20;
            Item.height = 30;
        }

        public override void AddRecipes()
        {

        }
    }
}