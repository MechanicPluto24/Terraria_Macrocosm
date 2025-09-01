using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Torches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Cheese
{
    public class CheeseCandelabra : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Cheese.CheeseCandelabra>());
            Item.width = 32;
            Item.height = 30;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CheeseBlock>(5)
                .AddIngredient<LuminiteTorch>(3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
