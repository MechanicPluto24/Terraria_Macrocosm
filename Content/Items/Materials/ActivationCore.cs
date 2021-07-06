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
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.value = 100;
            item.rare = 1;

        }
    }
}