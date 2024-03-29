using Macrocosm.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Placeable.Walls
{
    public class MoonBasePlatingWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;

            Item.createWall = WallType<Tiles.Walls.MoonBasePlatingWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<MoonBasePlating>()
                .AddTile(TileID.WorkBenches)
                .DisableDecraft()
                .AddCustomShimmerResult(ItemType<MoonBasePlatingWallUnsafe>())
                .Register();
        }
    }

    public class MoonBasePlatingWallUnsafe : MoonBasePlatingWall
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
            Item.createWall = WallType<Tiles.Walls.MoonBasePlatingWallUnsafe>();
        }

        public override void AddRecipes()
        {
        }
    }
}