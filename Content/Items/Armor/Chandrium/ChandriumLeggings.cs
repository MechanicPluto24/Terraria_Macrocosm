using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Chandrium
{
    [AutoloadEquip(EquipType.Legs)]
    public class ChandriumLeggings : ModItem
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

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.moveSpeed += 0.1f;
            player.GetModPlayer<MacrocosmPlayer>().SpaceProtection += 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ChandriumBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}