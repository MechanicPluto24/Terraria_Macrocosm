using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Drops
{
    public class CortexFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemSets.UnobtainableItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Red;

        }
    }
}