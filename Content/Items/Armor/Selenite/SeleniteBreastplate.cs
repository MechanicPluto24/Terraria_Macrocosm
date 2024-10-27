// using Macrocosm.Tiles;
using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Selenite
{
    [AutoloadEquip(EquipType.Body)]
    public class SeleniteBreastplate : ModItem
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
            Item.defense = 11;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<RangedDamageClass>() += 12f;
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SeleniteBar>(16)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}