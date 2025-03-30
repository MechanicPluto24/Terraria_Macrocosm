using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Plants
{
    public class RubberTreeSapling : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Trees.RubberTreeSapling>(), 0);
            Item.width = 16;
            Item.height = 36;
            Item.value = 10;
        }
    }
}
