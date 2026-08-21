using Macrocosm.Common.Bases.Items;
using Macrocosm.Content.Liquids;
using ModLiquidLib.ModLoader;

namespace Macrocosm.Content.Items.LiquidContainers;

public class OilBucket : Bucket
{
    public override int BucketLiquidType => LiquidLoader.LiquidType<Oil>();
}
