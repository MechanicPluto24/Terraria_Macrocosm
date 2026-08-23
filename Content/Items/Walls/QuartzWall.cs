using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls;

public class QuartzWall : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<QuartzWallUnsafe>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(VariantWall.WallType<Content.Walls.QuartzWall>());
        Item.width = 24;
        Item.height = 24;
    }

    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<QuartzFragment>()
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

public class QuartzWallUnsafe : QuartzWall
{
    public override string Texture => base.Texture.Replace("Unsafe", "");

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<QuartzWall>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.createWall = VariantWall.WallType<Content.Walls.QuartzWall>(WallSafetyType.Unsafe);
    }
}
