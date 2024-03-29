using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static bool BossActive => Main.CurrentFrameFlags.AnyActiveBossNPC;

        public static bool InvastionActive => Main.invasionType > 0 || Main.snowMoon || Main.pumpkinMoon || DD2Event.Ongoing;

        public static bool BloodMoonActive => Main.bloodMoon;

        public static bool MoonLordIncoming => NPC.MoonLordCountdown > 0;

        public static bool PillarsActive => Main.npc.Any(npc => npc.type is NPCID.LunarTowerVortex or NPCID.LunarTowerStardust or NPCID.LunarTowerNebula or NPCID.LunarTowerSolar);

        public static bool MartianProbeActive => Main.npc.Any(npc => npc.type is NPCID.MartianProbe);

    }
}
