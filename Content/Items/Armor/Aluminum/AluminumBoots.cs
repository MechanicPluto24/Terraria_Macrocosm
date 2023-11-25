using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor.Aluminum
{
    [AutoloadEquip(EquipType.Legs)]
    public class AluminumBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(silver: 6);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<AluminumBar>(), 18);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}