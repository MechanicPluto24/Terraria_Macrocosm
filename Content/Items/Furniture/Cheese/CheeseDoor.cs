using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Cheese
{
    public class CheeseDoor : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Cheese.CheeseDoorClosed>());
            Item.width = 20;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CheeseBlock>(6)
                .Register();
        }
    }
}
