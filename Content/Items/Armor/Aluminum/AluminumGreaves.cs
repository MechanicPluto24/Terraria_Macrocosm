using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Aluminum;

[AutoloadEquip(EquipType.Legs)]
[LegacyName("AluminumBoots")]
public class AluminumGreaves : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 18;
        Item.value = Item.sellPrice(silver: 6);
        Item.rare = ItemRarityID.White;
        Item.defense = 2;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<AluminumBar>(18)
        .AddTile(TileID.Anvils)
        .Register();
    }
}