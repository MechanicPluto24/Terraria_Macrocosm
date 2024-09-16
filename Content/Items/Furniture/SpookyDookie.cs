using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture
{
    public class SpookyDookie : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.SpookyDookie>());
            Item.width = 16;
            Item.height = 22;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
        }
    }
}
