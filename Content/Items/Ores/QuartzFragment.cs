using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Ores
{
    public class QuartzFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 1000;
            Item.rare = ModContent.RarityType<MoonRarity1>();
            
        }
    }
}