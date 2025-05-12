using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class SakuraRuckenfigur : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.SakuraRuckenfigur>());
            Item.width = 20;
            Item.height = 32;
        }

        public override void AddRecipes()
        {

        }
    }
}