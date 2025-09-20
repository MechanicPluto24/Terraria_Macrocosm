using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Drops;

public class BrokenHeroShield : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 38;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;


    }
}