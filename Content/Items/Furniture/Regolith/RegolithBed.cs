using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Regolith
{
    public class RegolithBed : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Regolith.RegolithBed>());
            Item.width = 34;
            Item.height = 22;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RegolithBrick>(15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
