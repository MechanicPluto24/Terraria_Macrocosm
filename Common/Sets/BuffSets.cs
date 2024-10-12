using Macrocosm.Common.Enums;
using System.Linq;
using Terraria.ID;

namespace Macrocosm.Common.Sets
{
    /// <summary>
    /// Buff/Debuff Sets for special behavior of some buffs/debuffs, useful for crossmod.
    /// Note: Only initalize sets with vanilla content here, add modded content to sets in SetStaticDefaults.
    /// </summary>
    public class BuffSets
    {
        /// <summary> Typical duration of buff/debuff. Usually used when applied randomly from a pool </summary>
        public static int[] TypicalDuration { get; } = BuffID.Sets.Factory.CreateIntSet();

        /// <summary> The severity of a radiation-related debuff. </summary>
        public static RadiationSeverity[] RadiationBuffSeverity { get; } = BuffID.Sets.Factory.CreateCustomSet(defaultState: RadiationSeverity.None);

        /// <summary> Get all radiation debuff IDs </summary>
        public static int[] GetRadiationDebuffs() => RadiationBuffSeverity
                .Select((buffSeverity, index) => new { buffSeverity, index })
                .Where(x => x.buffSeverity > RadiationSeverity.None)
                .Select(x => x.index)
                .ToArray();

        /// <summary> Get all radiation debuff IDs of a given severity </summary>
        public static int[] GetRadiationDebuffs(RadiationSeverity severity) => RadiationBuffSeverity
                .Select((buffSeverity, index) => new { buffSeverity, index })
                .Where(x => x.buffSeverity == severity)
                .Select(x => x.index)
                .ToArray();
    }
}
