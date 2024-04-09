using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Crafting
{
    public class OilRefinery : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Crafting.OilRefinery>());
            Item.width = 42;
            Item.height = 32;
            Item.value = 500;
        }
    }
}