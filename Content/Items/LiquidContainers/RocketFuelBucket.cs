using Macrocosm.Common.Bases.Items;
using Macrocosm.Content.Liquids;
using ModLiquidLib.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers;

public class RocketFuelBucket : Bucket
{
    public override int BucketLiquidType => LiquidLoader.LiquidType<RocketFuel>();

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
    }
}
