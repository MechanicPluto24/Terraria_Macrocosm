using Macrocosm.Content.Items.Materials.Bars;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Artemite
{
    [AutoloadEquip(EquipType.Head)]
    public class ArtemiteHelmet : ModItem
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
            Item.defense = 8;
        }

        public override void Load()
        {
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MeleeDamageClass>() += 0.18f;
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1.5f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<ArtemiteHelmet>() && body.type == ModContent.ItemType<ArtemiteBreastplate>() && legs.type == ModContent.ItemType<ArtemiteLeggings>();
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
}