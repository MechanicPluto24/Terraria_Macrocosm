using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class Retributor : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.Retributor>());
            Item.width = 22;
            Item.height = 32;
        }

        public override void AddRecipes()
        {

        }
    }
}