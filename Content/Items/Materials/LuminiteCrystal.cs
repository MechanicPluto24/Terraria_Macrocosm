using Macrocosm.Content.Items.Materials.Chunks;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
    public class LuminiteCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            // Set other Item.X values here
        }

        public override void AddRecipes()
        {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient<SidusChunk>(10);
            recipe.AddIngredient<NubisChunk>(10);
            recipe.AddIngredient<CinisChunk>(10);
            recipe.AddIngredient<TurbenChunk>(10);
            recipe.AddIngredient<CosmicEssence>(3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}