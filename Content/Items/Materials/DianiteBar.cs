using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Materials
{
    public class DianiteBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = 750;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = TileType<Tiles.DianiteBar>();
            Item.placeStyle = 0;
            // Set other Item.X values here
        }

        public override void AddRecipes()
        {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient<DianiteOre>(6);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}