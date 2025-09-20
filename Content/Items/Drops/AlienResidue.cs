using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Drops;

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
        Item.rare = ModContent.RarityType<MoonRarity1>();

    }
}