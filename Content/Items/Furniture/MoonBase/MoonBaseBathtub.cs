using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;


namespace Macrocosm.Content.Items.Furniture.MoonBase
{
    public class MoonBaseBathtub : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseBathtub>());
            Item.width = 30;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MoonBasePlating>(14)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}
