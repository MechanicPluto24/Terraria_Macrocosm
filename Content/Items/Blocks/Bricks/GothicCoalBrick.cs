using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks
{
    public class GothicCoalBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.GothicCoalBrick>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Coal>(2)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
        }
    }
}