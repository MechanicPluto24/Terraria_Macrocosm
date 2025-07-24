using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Steel;

[AutoloadEquip(EquipType.Legs)]
public class SteelLeggings : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 16;
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 4;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<SteelBar>(15)
        .AddTile(TileID.Anvils)
        .Register();
    }
}