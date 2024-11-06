using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture
{
    public class XFLR6 : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.XFLR6>());
            Item.width = 26;
            Item.height = 32;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AluminumBar>(10)
                .AddIngredient<Plastic>(25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}