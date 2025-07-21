using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Furniture.Industrial;

namespace Macrocosm.Content.Items.Blocks
{
    public class IndustrialPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.IndustrialPlating>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient<Bars.AluminumBar>(1)
                .AddIngredient<Bars.SteelBar>(1)
                .AddIngredient(ItemID.StoneBlock, 5)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<IndustrialPlatform>(2)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<IndustrialPlatingWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<IndustrialHazardWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<IrradiatedRockWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<IndustrialTrimmingWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}