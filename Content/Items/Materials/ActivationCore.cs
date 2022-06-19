using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
    public class ActivationCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("There are large amounts of energy emnating from within the core, maybe you could ");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 100;
            Item.rare = 1;

        }
    }
}