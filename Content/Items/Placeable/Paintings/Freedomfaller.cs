using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Paintings
{
    public class Freedomfaller : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.Freedomfaller>());
            Item.width = 26;
            Item.height = 26;
        }

        public override void AddRecipes()
        {

        }
    }
}