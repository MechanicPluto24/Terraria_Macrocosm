using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Blocks.Terrain;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Walls;

public class AstrolithWall : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 400;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AstrolithWallUnsafe>();
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(VariantWall.WallType<Content.Walls.AstrolithWall>());
        Item.width = 24;
        Item.height = 24;
    }

    public override void AddRecipes()
    {
        CreateRecipe(4)
            .AddIngredient<Astrolith>()
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

public class AstrolithWallUnsafe : AstrolithWall
{
    public override string Texture => base.Texture.Replace("Unsafe", "");

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AstrolithWall>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.createWall = VariantWall.WallType<Content.Walls.AstrolithWall>(WallSafetyType.Unsafe);
    }
}
