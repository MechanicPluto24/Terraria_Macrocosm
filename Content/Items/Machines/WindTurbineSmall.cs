using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class WindTurbineSmall : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.WindTurbineSmall>());
            Item.width = 20;
            Item.height = 48;
            Item.value = 500;
            Item.mech = true;
        }
    }
}