using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Miscellaneous
{
    public class StarshipKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("This could take you to a spaceship subworld, if we knew how to make subworlds."); // FIXME: Please, dude (kirito) please this is cancer
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {

            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient<ActivationCore>();
            recipe.AddIngredient<UnpoweredKey>();
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}