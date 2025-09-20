using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Fishes;

public class Craterfish : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 2;
        ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        Item.value = Item.sellPrice(silver: 1);
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.maxStack = Item.CommonMaxStack;
    }
}
