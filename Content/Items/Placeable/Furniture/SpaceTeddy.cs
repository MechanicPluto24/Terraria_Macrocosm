using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
    public class SpaceTeddy : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.TeddyBear>(), tileStyleToPlace: 1);
            Item.width = 20;
            Item.height = 28;
            Item.value = 500;
        }
    }
}