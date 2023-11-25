using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools.Aluminum
{
    public class AluminumPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Melee;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 21;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(silver: 3, copper: 25);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.pick = 38;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient<AluminumBar>(10);
            recipe.AddRecipeGroup(RecipeGroupID.Wood, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}