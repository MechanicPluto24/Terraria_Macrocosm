using Macrocosm.Content.Items.Materials.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Aluminum
{
    [AutoloadEquip(EquipType.Body)]
    public class AluminumBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 8);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<AluminumBar>(22)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}