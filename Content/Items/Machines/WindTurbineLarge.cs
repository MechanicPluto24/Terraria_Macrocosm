using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class WindTurbineLarge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.WindTurbineLarge>());
            Item.width = 28;
            Item.height = 56;
            Item.value = 500;
            Item.mech = true;
        }
    }
}