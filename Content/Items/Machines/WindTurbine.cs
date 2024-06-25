using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class WindTurbine : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.WindTurbine>());
            Item.width = 36;
            Item.height = 84;
            Item.value = 500;
            Item.mech = true;
        }
    }
}