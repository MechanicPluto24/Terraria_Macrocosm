using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Vanity.Employee
{
    [AutoloadEquip(EquipType.Legs)]
    public class EmployeeBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.value = Item.sellPrice(silver: 200);
            Item.rare = ModContent.RarityType<MoonRarity1>();
            Item.vanity = true;
        }

        public override void AddRecipes()
        {
        }
    }
}