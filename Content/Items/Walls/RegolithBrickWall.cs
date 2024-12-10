using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls
{
    public class RegolithBrickWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<RegolithBrickWallUnsafe>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Tiles.Walls.RegolithBrickWall>());
            Item.width = 24;
            Item.height = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<RegolithBrick>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class RegolithBrickWallUnsafe : RegolithBrickWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<RegolithBrickWall>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<Tiles.Walls.RegolithBrickWallUnsafe>();
        }
    }
}