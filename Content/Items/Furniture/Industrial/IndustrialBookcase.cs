using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Industrial
{
    public class IndustrialBookcase : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Industrial.IndustrialBookcase>(), tileStyleToPlace: 0);
            Item.width = 28;
            Item.height = 34;
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
                .AddIngredient<IndustrialPlating>(20)
                .AddIngredient(ItemID.Book, 10)
                .Register();
        }
    }
}
