using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace Macrocosm.Common.Utils;

public static partial class Utility
{
    public static bool BossActive => Main.CurrentFrameFlags.AnyActiveBossNPC;

    public static bool InvastionActive => Main.invasionType > 0 || Main.snowMoon || Main.pumpkinMoon || DD2Event.Ongoing;

    public static bool BloodMoonActive => Main.bloodMoon;

    public static bool MoonLordIncoming => NPC.MoonLordCountdown > 0;

    public static bool PillarsActive => Main.npc.Any(npc => npc.type is NPCID.LunarTowerVortex or NPCID.LunarTowerStardust or NPCID.LunarTowerNebula or NPCID.LunarTowerSolar);

    public static bool MartianProbeActive => Main.npc.Any(npc => npc.type is NPCID.MartianProbe);

    public static void WorldGen_ShakeTree(int i, int j) => typeof(WorldGen).InvokeMethod("ShakeTree", parameters: [i, j]);

    public static int WorldGen_numTreeShakes
    {
        get => typeof(WorldGen).GetFieldValue<int>("numTreeShakes");
        set => typeof(WorldGen).SetFieldValue("numTreeShakes", value);
    }

    public static int WorldGen_maxTreeShakes
    {
        get => typeof(WorldGen).GetFieldValue<int>("maxTreeShakes");
        set => typeof(WorldGen).SetFieldValue("maxTreeShakes", value);
    }

    public static (int[] treeShakeX, int[] treeShakeY) WorldGen_treeShakeXY
        => (typeof(WorldGen).GetFieldValue<int[]>("treeShakeX"), typeof(WorldGen).GetFieldValue<int[]>("treeShakeY"));
}
