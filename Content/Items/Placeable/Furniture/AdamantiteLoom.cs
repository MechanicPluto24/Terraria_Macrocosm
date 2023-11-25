using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
    public class AdamantiteLoom : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.IndustrialLoom>());
            Item.placeStyle = 1;

            Item.width = 50;
            Item.height = 24;
            Item.value = 500;
        }
    }
}