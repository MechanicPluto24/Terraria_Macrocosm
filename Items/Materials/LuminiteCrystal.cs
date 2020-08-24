using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Items.Materials
{
    public class LuminiteCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = 100;
            item.rare = 1;
            // Set other item.X values here
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(mod, "SidusChunk", 10);
            recipe.AddIngredient(mod, "NubisChunk", 10);
            recipe.AddIngredient(mod, "CinisChunk", 10);
            recipe.AddIngredient(mod, "TurbenChunk", 10);
            recipe.AddIngredient(mod, "CosmicEssence", 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}