using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls
{
    public class IndustrialSquarePaneledWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Tiles.Walls.IndustrialSquarePaneledWall>());
            Item.width = 24;
            Item.height = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<IndustrialPlating>()
                .AddTile(TileID.WorkBenches)
                .DisableDecraft()
                .AddCustomShimmerResult(ModContent.ItemType<IndustrialSquarePaneledWallUnsafe>())
                .Register();
        }
    }

    public class IndustrialSquarePaneledWallUnsafe : IndustrialSquarePaneledWall
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
            Item.createWall = ModContent.WallType<Tiles.Walls.IndustrialSquarePaneledWallUnsafe>();
        }

        public override void AddRecipes()
        {
        }
    }
}