using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class OilRefinery : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.OilRefinery>());
            Item.width = 42;
            Item.height = 32;
            Item.value = 500;
            Item.mech = true;
        }
    }
}