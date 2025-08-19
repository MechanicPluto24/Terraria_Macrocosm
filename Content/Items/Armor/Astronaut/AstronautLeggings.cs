// using Macrocosm.Tiles;
using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Tiles.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Astronaut;

[AutoloadEquip(EquipType.Legs)]
public class AstronautLeggings : ModItem
{
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.rare = ItemRarityID.Purple;
        Item.defense = 6;
    }

    public override void UpdateEquip(Player player)
    {
        player.moveSpeed += 0.05f;
        player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
        .AddIngredient<SpacesuitFabric>(20)
        .AddTile<IndustrialLoom>()
        .Register();
    }
}