using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class SolarPanelSmall : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.SolarPanelSmall>());
            Item.width = 44;
            Item.height = 34;
            Item.value = 500;
        }
    }
}