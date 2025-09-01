// using Macrocosm.Tiles;
using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Dianite;

[AutoloadEquip(EquipType.Body)]
public class DianiteBreastplate : ModItem
{
   
    private int backSlot;
    private int frontSlot;
    public override void Load()
    {
        backSlot = EquipLoader.AddEquipTexture(Mod, Texture + "_Back", EquipType.Back, name: "DianiteCapeB");
        frontSlot = EquipLoader.AddEquipTexture(Mod, Texture + "_Front", EquipType.Front, name: "DianiteCapeF");
    }
    public override void SetStaticDefaults()
    {
        ArmorIDs.Body.Sets.IncludedCapeBack[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = backSlot;
        ArmorIDs.Body.Sets.IncludedCapeBackFemale[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = backSlot;
        ArmorIDs.Body.Sets.IncludedCapeFront[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = frontSlot;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 10);
        Item.rare = ModContent.RarityType<MoonRarity1>();
        Item.defense = 11;
    }

    public override void UpdateEquip(Player player)
    {
        player.GetCritChance<MagicDamageClass>() += 12f;
        player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<DianiteBar>(16)
        .AddTile(TileID.LunarCraftingStation)
        .Register();
    }
}