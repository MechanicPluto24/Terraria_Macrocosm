using Macrocosm.Common.Bases.Items;
using Macrocosm.Content.Liquids;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers;

public class OilBucket : Bucket
{
    public override int BucketLiquidType => LiquidLoader.LiquidType<Oil>();

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        LiquidID_TLmod.Sets.CreateLiquidBucketItem[BucketLiquidType] = Type;
        base.SetDefaults();
    }
}
