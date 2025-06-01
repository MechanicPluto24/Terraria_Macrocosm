using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class CacoLisa : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.CacoLisa>());
            Item.width = 34;
            Item.height = 50;
        }

        public override void AddRecipes()
        {

        }
    }
}