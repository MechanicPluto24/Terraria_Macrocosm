using System.Linq;
using System.Reflection;
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


        private static MethodInfo worldGen_ShakeTree_methodInfo;
        private static FieldInfo worldGen_numTreeShakes_fieldInfo;
        private static FieldInfo worldGen_maxTreeShakes_fieldInfo;
        private static FieldInfo worldGen_treeShakeX_fieldInfo;
        private static FieldInfo worldGen_treeShakeY_fieldInfo;

        public static void WorldGen_ShakeTree(int i, int j)
        {
            worldGen_ShakeTree_methodInfo ??= typeof(WorldGen).GetMethod("ShakeTree", BindingFlags.NonPublic | BindingFlags.Static);
            worldGen_ShakeTree_methodInfo.Invoke(null, [i, j]);
        }

        public static int WorldGen_numTreeShakes
        {
            get
            {
                worldGen_numTreeShakes_fieldInfo ??= typeof(WorldGen).GetField("numTreeShakes", BindingFlags.NonPublic | BindingFlags.Static);
                return (int)worldGen_numTreeShakes_fieldInfo.GetValue(null);
            }
            set
            {
                worldGen_numTreeShakes_fieldInfo ??= typeof(WorldGen).GetField("numTreeShakes", BindingFlags.NonPublic | BindingFlags.Static);
                worldGen_numTreeShakes_fieldInfo.SetValue(null, value);
            }
        }

        public static int WorldGen_maxTreeShakes
        {
            get
            {
                worldGen_maxTreeShakes_fieldInfo ??= typeof(WorldGen).GetField("maxTreeShakes", BindingFlags.NonPublic | BindingFlags.Static);
                return (int)worldGen_maxTreeShakes_fieldInfo.GetValue(null);
            }
            set
            {
                worldGen_maxTreeShakes_fieldInfo ??= typeof(WorldGen).GetField("maxTreeShakes", BindingFlags.NonPublic | BindingFlags.Static);
                worldGen_maxTreeShakes_fieldInfo.SetValue(null, value);
            }

        }

        public static (int[] treeShakeX, int[] treeShakeY) WorldGen_treeShakeXY
        {
            get
            {
                worldGen_treeShakeX_fieldInfo ??= typeof(WorldGen).GetField("treeShakeX", BindingFlags.NonPublic | BindingFlags.Static);
                worldGen_treeShakeY_fieldInfo ??= typeof(WorldGen).GetField("treeShakeY", BindingFlags.NonPublic | BindingFlags.Static);
                return ((int[])worldGen_treeShakeX_fieldInfo.GetValue(null), (int[])worldGen_treeShakeY_fieldInfo.GetValue(null));
            }
        }
    }
}
