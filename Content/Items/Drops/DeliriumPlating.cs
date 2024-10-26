using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Drops
{
    public class DeliriumPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value =  Item.sellPrice(gold:1);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.material = true;

        }
    }
}