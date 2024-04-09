using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Special
{
    public class LaunchPadMarker : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Special.LaunchPadMarker>());

            Item.width = 16;
            Item.height = 16;
            Item.value = 500;
        }

        public override bool? UseItem(Player player)
        {
            return null;
        }
    }
}
