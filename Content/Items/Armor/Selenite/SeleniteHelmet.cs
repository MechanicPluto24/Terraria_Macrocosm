using Macrocosm.Common.Players;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Selenite
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("ArtemiteHelmet")]
    public class SeleniteHelmet : ModItem
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
            Item.defense = 8;
        }

        public override void Load()
        {
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.18f;
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<SeleniteHelmet>() && body.type == ModContent.ItemType<SeleniteBreastplate>() && legs.type == ModContent.ItemType<SeleniteLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SeleniteBar>(8)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}