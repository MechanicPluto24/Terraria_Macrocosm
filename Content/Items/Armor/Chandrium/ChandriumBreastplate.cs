using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Chandrium;

[AutoloadEquip(EquipType.Body)]
public class ChandriumBreastplate : ModItem
{

    private int backSlot;
    public override void Load()
    {
        backSlot = EquipLoader.AddEquipTexture(Mod, Texture + "_Back", EquipType.Back, name: "ChandriumCape");
    }
    public override void SetStaticDefaults()
    {
        ArmorIDs.Body.Sets.IncludedCapeBack[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = backSlot;
        ArmorIDs.Body.Sets.IncludedCapeBackFemale[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = backSlot;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.defense = 9;
    }

    public override void UpdateEquip(Player player)
    {
        player.maxMinions += 1;
        player.whipRangeMultiplier += 0.15f;
        player.GetAttackSpeed<SummonDamageClass>() += 0.15f;
        player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<ChandriumBar>(16)
        .AddTile(TileID.LunarCraftingStation)
        .Register();
    }
}