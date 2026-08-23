using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Items;
public class VanillaRecipeItem : ModSystem
{
    public override void AddRecipes()
    {
        Recipe wireRecipe = Recipe.Create(ItemID.Wire, 10);
        wireRecipe.AddIngredient(ItemID.CopperBar, 2);
        wireRecipe.AddIngredient<Rubber>();
        wireRecipe.AddTile(TileID.Anvils);
        wireRecipe.Register();
    }
}
