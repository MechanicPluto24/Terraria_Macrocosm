using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Vanity.Employee;

[AutoloadEquip(EquipType.Body)]
public class EmployeeSuit : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 18;
        Item.value = Item.sellPrice(silver: 150);
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.vanity = true;
    }

    public override void AddRecipes()
    {
    }
}