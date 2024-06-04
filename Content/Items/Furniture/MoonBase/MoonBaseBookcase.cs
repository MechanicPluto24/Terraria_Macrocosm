using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.MoonBase
{
    public class MoonBaseBookcase : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseBookcase>(), tileStyleToPlace: 0);
            Item.width = 28;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 150;
        }

        public override bool? UseItem(Player player)
        {
            if(Main.rand.NextBool(5))
                Item.placeStyle = 1;
            else
                Item.placeStyle = 0;

            return base.UseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MoonBasePlating>(20)
                .AddIngredient(ItemID.Book, 10)
                .Register();
        }
    }
}
