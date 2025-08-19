using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Dianite;

[AutoloadEquip(EquipType.Head)]
public class DianiteVisor : ModItem
{
    public override void SetStaticDefaults()
    {
    }
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.defense = 5;
    }

    public override void UpdateEquip(Player player)
    {
        player.statManaMax2 += 60;
        player.GetDamage<MagicDamageClass>() += 0.1f;
        player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == ModContent.ItemType<DianiteVisor>() && body.type == ModContent.ItemType<DianiteBreastplate>() && legs.type == ModContent.ItemType<DianiteLeggings>();
    }

    public override void UpdateArmorSet(Player player)
    {
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<DianiteBar>(8)
        .AddTile(TileID.LunarCraftingStation)
        .Register();
    }
}