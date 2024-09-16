using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class BurnerGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.BurnerGenerator>());
            Item.width = 52;
            Item.height = 30;
            Item.value = 500;
            Item.mech = true;
        }
    }
}