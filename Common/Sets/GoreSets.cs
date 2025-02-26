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
        public static float[] TrashnadoChance { get; } = GoreID.Sets.Factory.CreateFloatSet(defaultState: 0f,
            99, 1f,
            268, 1f,
            GoreID.SkeletonMerchantBag, 1f
        );
    }
}
