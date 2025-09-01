using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Blocks.Terrain;

namespace Macrocosm.Content.Items.Blocks.Bricks
{
    public class SeleniteBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.SeleniteBrick>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
            .AddIngredient<SeleniteOre>(1)
            .AddIngredient<Protolith>(4)
            .AddTile(TileID.Furnaces)
            .Register();
        }
    }
}