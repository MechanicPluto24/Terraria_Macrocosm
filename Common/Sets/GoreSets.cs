using Macrocosm.Common.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Macrocosm.Common.Sets
{
    /// <summary>
    /// Gore Sets for special behavior of some Gores, useful for crossmod.
    /// Note: Only initalize sets with vanilla content here, add modded content to sets in SetStaticDefaults.
    /// </summary>
    public class GoreSets
    {
        public static TrashData[] TrashData { get; } = GoreID.Sets.Factory.CreateCustomSet(defaultState: new TrashData(),
            99, new TrashData(type: 99, dustType: 54),
            269, new TrashData(type: 269, dustType: DustID.Bone),
            GoreID.SkeletonMerchantBag, new TrashData(GoreID.SkeletonMerchantBag, 7)
        );
    }
}
