using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Paintings
{
    public class IsleOfTheDead : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.IsleOfTheDead>());
            Item.width = 40;
            Item.height = 28;
        }

        public override void AddRecipes()
        {

        }
    }
}