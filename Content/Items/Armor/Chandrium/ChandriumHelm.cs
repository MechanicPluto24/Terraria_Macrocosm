using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Chandrium;

[AutoloadEquip(EquipType.Head)]
public class ChandriumHelm : ModItem
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
        Item.defense = 4;
    }

    public override void UpdateEquip(Player player)
    {
        player.maxMinions += 1;
        player.GetDamage<SummonDamageClass>() += 0.25f;
        player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == ModContent.ItemType<ChandriumHelm>() && body.type == ModContent.ItemType<ChandriumBreastplate>() && legs.type == ModContent.ItemType<ChandriumLeggings>();
    }

    public override void UpdateArmorSet(Player player)
    {
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<ChandriumBar>(8)
        .AddTile(TileID.LunarCraftingStation)
        .Register();
    }
}