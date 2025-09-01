using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Steel;

[AutoloadEquip(EquipType.Body)]
public class SteelChestplate : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 20;
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 5;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<SteelBar>(20)
        .AddTile(TileID.Anvils)
        .Register();
    }
}