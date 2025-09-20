using Macrocosm.Common.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Sets;

/// <summary> Gore Sets for special behavior of some Gores, useful for crossmod. </summary>
[ReinitializeDuringResizeArrays]
public class GoreSets
{
    public static TrashData[] TrashData { get; } = GoreID.Sets.Factory.CreateCustomSet(defaultState: new TrashData(),
        99, new TrashData(type: 99, dustType: 54),
        269, new TrashData(type: 269, dustType: DustID.Bone),
        GoreID.SkeletonMerchantBag, new TrashData(GoreID.SkeletonMerchantBag, 7)
    );
}
