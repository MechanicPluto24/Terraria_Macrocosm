using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Paintings
{
    public class Apollo115 : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.Apollo115>());
            Item.width = 50;
            Item.height = 34;
        }

        public override void AddRecipes()
        {

        }
    }
}