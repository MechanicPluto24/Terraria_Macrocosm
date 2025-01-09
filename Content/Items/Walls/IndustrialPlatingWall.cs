using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls
{
    [LegacyName("MoonBasePlatingWall")]
    public class IndustrialPlatingWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<IndustrialPlatingWallUnsafe>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Tiles.Walls.IndustrialPlatingWall>());
            Item.width = 24;
            Item.height = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<IndustrialPlating>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [LegacyName("MoonBasePlatingWallUnsafe")]
    public class IndustrialPlatingWallUnsafe : IndustrialPlatingWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<IndustrialPlatingWall>();

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<Tiles.Walls.IndustrialPlatingWallUnsafe>();
        }
    }
}