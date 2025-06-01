using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Sets
{
    /// <summary> NPC Sets for special behavior of some NPCs, useful for crossmod.  </summary>
    [ReinitializeDuringResizeArrays]
    public class NPCSets
    {
        /// <summary> NPC types that can spawn on the Moon. Also adds the Moon tag to the bestiary. </summary>
        public static bool[] MoonNPC { get; } = NPCID.Sets.Factory.CreateNamedSet(nameof(MoonNPC)).Description("NPC types that can spawn on the Moon. Also adds the Moon tag to the bestiary.").RegisterBoolSet();

        /// <summary> NPCs that spawn on the Mars. Also adds the Mars tag to the bestiary. </summary>
        // public static bool[] MarsNPC { get; } = NPCID.Sets.Factory.CreateBoolSet();

        // ...

        /// <summary> NPC types that don't drop Moonstone if killed on the Moon. </summary>
        public static bool[] NoMoonstoneDrop { get; } = NPCID.Sets.Factory.CreateNamedSet(nameof(NoMoonstoneDrop)).Description("NPC types that don't drop Moonstone if killed on the Moon.").RegisterBoolSet();
    }
}
