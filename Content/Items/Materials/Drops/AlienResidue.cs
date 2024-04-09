using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Drops
{
    public class AlienResidue : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.material = true;
        }
    }
}