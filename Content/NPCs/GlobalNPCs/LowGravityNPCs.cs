using Terraria.ModLoader;
using Terraria;
using SubworldLibrary;
using System.Reflection;
using Macrocosm.Content.Subworlds.Moon;

namespace Macrocosm.Content.NPCs.GlobalNPCs {
    public class LowGravityNPC : GlobalNPC {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public static FieldInfo NPCGravity { get; private set; }

        public static void DetourNPCGravity() {
            NPCGravity = typeof(NPC).GetField("gravity", BindingFlags.NonPublic | BindingFlags.Static);
            On.Terraria.NPC.UpdateNPC_UpdateGravity += NPC_UpdateNPC_UpdateGravity;
        }

        private static void NPC_UpdateNPC_UpdateGravity(On.Terraria.NPC.orig_UpdateNPC_UpdateGravity orig, NPC self, out float maxFallSpeed) {
            orig(self, out maxFallSpeed);
            if (SubworldSystem.IsActive<Moon>())
                NPCGravity.SetValue(null, 0.05f);
        }
    }
}