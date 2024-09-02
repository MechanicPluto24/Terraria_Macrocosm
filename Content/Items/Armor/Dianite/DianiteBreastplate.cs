// using Macrocosm.Tiles;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Dianite
{
    [AutoloadEquip(EquipType.Body)]
    public class DianiteBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
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
}