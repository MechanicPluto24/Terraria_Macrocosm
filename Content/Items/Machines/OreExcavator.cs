using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class OreExcavator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.OreExcavator>());
            Item.width = 60;
            Item.height = 84;
            Item.value = 500;
            Item.mech = true;
        }

        public override void AddRecipes()
        {
        }
    }
}