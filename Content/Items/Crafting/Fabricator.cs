using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Crafting
{
    public class Fabricator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Crafting.OilRefinery>());
            Item.width = 48;
            Item.height = 50;
            Item.value = 500;
        }
    }
}