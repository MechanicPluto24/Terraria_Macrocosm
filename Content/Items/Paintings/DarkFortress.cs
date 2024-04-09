using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class DarkFortress : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.DarkFortress>());
            Item.width = 32;
            Item.height = 32;
        }

        public override void AddRecipes()
        {

        }
    }
}