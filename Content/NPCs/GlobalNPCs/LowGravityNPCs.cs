using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.Reflection;

namespace Macrocosm.Content.NPCs.GlobalNPCs
{
    public class LowGravityNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public static FieldInfo NPCGravity { get; private set; }

        public static void DetourNPCGravity()
        {
            NPCGravity = typeof(NPC).Assembly.GetType("Terraria.NPC").GetField("gravity", BindingFlags.NonPublic | BindingFlags.Static);
            On.Terraria.NPC.UpdateNPC_UpdateGravity += NPC_UpdateNPC_UpdateGravity;
        }

        private static void NPC_UpdateNPC_UpdateGravity(On.Terraria.NPC.orig_UpdateNPC_UpdateGravity orig, NPC self, out float maxFallSpeed)
        {
            if (Subworld.IsActive<Moon>())
                NPCGravity.SetValue(null, 0.05f);
            orig(self, out maxFallSpeed);
        }
    }
}