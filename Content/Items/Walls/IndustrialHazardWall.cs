using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls
{
    public class IndustrialHazardWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<IndustrialHazardWallUnsafe>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Content.Walls.IndustrialHazardWall>());
            Item.width = 22;
            Item.height = 22;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<IndustrialPlating>()
                .AddTile(TileID.WorkBenches)
                .AddCustomShimmerResult(ModContent.ItemType<IndustrialHazardWallUnsafe>(), 4)
                .Register();
        }
    }

    public class IndustrialHazardWallUnsafe : IndustrialHazardWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<IndustrialHazardWall>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = Utility.GetWallVariantType(Item.createWall, WallSafetyType.Unsafe);
        }
    }
}