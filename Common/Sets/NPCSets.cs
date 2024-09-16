using Terraria.ID;

namespace Macrocosm.Common.Sets
{
    /// <summary>
    /// NPC Sets for special behavior of some NPCs, useful for crossmod.
    /// Note: Only initalize sets with vanilla content here, add modded content to sets in SetStaticDefaults.
    /// </summary>
    public class NPCSets
    {
        /// <summary> NPCs that can spawn on the Moon. Also adds the Moon tag to the bestiary. </summary>
        public static bool[] MoonNPC { get; } = NPCID.Sets.Factory.CreateBoolSet();

        /// <summary> NPCs that spawn on the Mars. Also adds the Mars tag to the bestiary. </summary>
        // public static bool[] MarsNPC { get; } = NPCID.Sets.Factory.CreateBoolSet();

        // ...

        /// <summary> Should the NPC drop moonstone while on the Moon </summary>
        public static bool[] DropsMoonstone { get; } = NPCID.Sets.Factory.CreateBoolSet();
    }
}
