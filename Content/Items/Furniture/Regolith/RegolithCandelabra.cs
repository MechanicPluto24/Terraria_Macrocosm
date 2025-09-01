using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Blocks.Bricks;
using Macrocosm.Content.Items.Torches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Regolith
{
    public class RegolithCandelabra : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Regolith.RegolithCandelabra>());
            Item.width = 24;
            Item.height = 28;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RegolithBrick>(5)
                .AddIngredient<LuminiteTorch>(3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
