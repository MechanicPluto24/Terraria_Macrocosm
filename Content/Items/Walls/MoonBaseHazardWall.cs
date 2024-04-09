using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls
{
    public class MoonBaseHazardWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Tiles.Walls.MoonBaseHazardWall>());
            Item.width = 22;
            Item.height = 22;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<MoonBasePlating>()
                .AddTile(TileID.WorkBenches)
                .DisableDecraft()
                .AddCustomShimmerResult(ModContent.ItemType<MoonBasePlatingWallUnsafe>())
                .Register();
        }
    }

    public class MoonBaseHazardWallUnsafe : MoonBaseHazardWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<Tiles.Walls.MoonBaseHazardWallUnsafe>();
        }

        public override void AddRecipes()
        {
        }
    }
}