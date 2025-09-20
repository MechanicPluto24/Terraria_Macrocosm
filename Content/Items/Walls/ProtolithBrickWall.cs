using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls;

public class ProtolithBrickWall : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ProtolithBrickWallUnsafe>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(VariantWall.WallType<Content.Walls.ProtolithBrickWall>());
        Item.width = 24;
        Item.height = 24;
    }

    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<ProtolithBrick>()
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

public class ProtolithBrickWallUnsafe : ProtolithBrickWall
{
    public override string Texture => base.Texture.Replace("Unsafe", "");

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ProtolithBrickWall>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.createWall = VariantWall.WallType<Content.Walls.ProtolithBrickWall>(WallSafetyType.Unsafe);
    }
}