using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Items.Currency
{
    public class CentuCredit : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("You'll want to keep this for a later update.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999999;
            item.value = 750;

            // Set other item.X values here
        }
    }
}