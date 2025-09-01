using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Dianite
{
    [AutoloadEquip(EquipType.Legs)]
    public class DianiteLeggings : ModItem
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
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.05f;
            player.moveSpeed += 0.1f;
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<DianiteBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}