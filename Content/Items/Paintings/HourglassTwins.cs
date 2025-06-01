using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class HourglassTwins : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.HourglassTwins>());
            Item.width = 28;
            Item.height = 28;
        }

        public override void AddRecipes()
        {

        }
    }
}