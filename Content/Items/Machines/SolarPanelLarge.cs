using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class SolarPanelLarge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.SolarPanelLarge>());
            Item.width = 44;
            Item.height = 34;
            Item.value = 500;
            Item.mech = true;
        }
    }
}