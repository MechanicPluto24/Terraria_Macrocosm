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
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.value = 100;
            item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {

            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod, "ActivationCore", 1);
            recipe.AddIngredient(mod, "UnpoweredKey", 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}