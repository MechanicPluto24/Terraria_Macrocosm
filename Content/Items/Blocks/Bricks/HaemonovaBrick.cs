using Macrocosm.Common.Conditions;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Flags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Bricks;

public class HaemonovaBrick : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
        ShimmerSystem.RegisterOverride(ItemID.LunarBrick, Type, CustomConditions.BloodMoonOrDemonSun);
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Bricks.HaemonovaBrick>());
    }

    public override void AddRecipes()
    {
    }
}