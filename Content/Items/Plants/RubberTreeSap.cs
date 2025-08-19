using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Plants;

public class RubberTreeSap : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = 500;
    }
}
