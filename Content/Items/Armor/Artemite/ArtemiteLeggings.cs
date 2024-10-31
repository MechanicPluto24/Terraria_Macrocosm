using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Artemite
{
    [AutoloadEquip(EquipType.Legs)]
    public class ArtemiteLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += 0.15f;
            player.moveSpeed += 0.15f;
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ArtemiteBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}