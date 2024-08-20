// using Macrocosm.Tiles;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Artemite
{
    [AutoloadEquip(EquipType.Body)]
    public class ArtemiteBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
        }


        public override void Load()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<MeleeDamageClass>() += 6f;
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ArtemiteBar>(16)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}