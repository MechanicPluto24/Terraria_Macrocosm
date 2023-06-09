using Macrocosm.Content.Items.Materials;
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
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() += 0.15f;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<ArtemiteBar>(), 12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}