using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Artemite;

[AutoloadEquip(EquipType.Head)]
[LegacyName("SeleniteHeadgear")]
public class ArtemiteHeadgear : ModItem
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
        Item.defense = 6;
    }
    public override void UpdateEquip(Player player)
    {
        var modPlayer = player.GetModPlayer<MacrocosmPlayer>();
        player.GetDamage<RangedDamageClass>() += 0.1f;
        modPlayer.ChanceToNotConsumeAmmo += 0.15f;
        player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return head.type == ModContent.ItemType<ArtemiteHeadgear>() && body.type == ModContent.ItemType<ArtemiteBreastplate>() && legs.type == ModContent.ItemType<ArtemiteLeggings>();
    }

    public override void UpdateArmorSet(Player player)
    {
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<ArtemiteBar>(8)
        .AddTile(TileID.LunarCraftingStation)
        .Register();
    }
}