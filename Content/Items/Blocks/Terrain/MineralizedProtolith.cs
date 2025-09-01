using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Terrain
{
    public class MineralizedProtolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Terrain.MineralizedProtolith>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient<Protolith>(10)
                .AddIngredient<LunarCrystal>(1)
                .AddTile(TileID.WorkBenches)
                .Register();

        }
    }
}