using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tech
{
    public class CompactDisk : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            ItemSets.UnobtainableItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
        }
    }
}