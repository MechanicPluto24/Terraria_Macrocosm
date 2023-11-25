using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
    public class SolarPanelSmall : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.SolarPanelSmall>());
            Item.width = 44;
            Item.height = 34;
            Item.value = 500;
        }
    }
}