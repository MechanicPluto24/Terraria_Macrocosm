using Macrocosm.Content.Items.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Steel;

[AutoloadEquip(EquipType.Head)]
public class SteelHelmet : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(silver: 80);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 5;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == ModContent.ItemType<SteelHelmet>() && body.type == ModContent.ItemType<SteelChestplate>() && legs.type == ModContent.ItemType<SteelLeggings>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.endurance += 0.1f;
        player.setBonus = "10% damage reduction"; // TODO: localize
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<SteelBar>(10)
        .AddTile(TileID.Anvils)
        .Register();
    }
}