using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class WhenDayBreaks : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.WhenDayBreaks>());
            Item.width = 32;
            Item.height = 22;
        }

        public override void AddRecipes()
        {
        }
    }
}