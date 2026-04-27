using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls;

public class CorrugatedSteelWall : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CorrugatedSteelWallUnsafe>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(VariantWall.WallType<Content.Walls.CorrugatedSteelWall>());
        Item.width = 24;
        Item.height = 24;
    }

    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<CorrugatedSteelBrick>()
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

public class CorrugatedSteelWallUnsafe : CorrugatedSteelWall
{
    public override string Texture => base.Texture.Replace("Unsafe", "");

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CorrugatedSteelWall>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.createWall = VariantWall.WallType<Content.Walls.CorrugatedSteelWall>(WallSafetyType.Unsafe);
    }
}
