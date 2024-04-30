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
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Crafting.Fabricator>());
            Item.width = 38;
            Item.height = 36;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
        }
    }
}