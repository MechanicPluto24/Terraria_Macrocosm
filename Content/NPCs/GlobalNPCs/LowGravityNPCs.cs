using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;

namespace Macrocosm.Content.NPCs.GlobalNPCs
{
    public class LowGravityNPC : GlobalNPC
    {
        public float gravity = 1f;
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public override void PostAI(NPC npc)
        {
            // wtf
            gravity = 1f;
            if (!npc.noGravity)
            {
                if (Subworld.IsActive<Moon>())
                    gravity = 0.166f;

                npc.velocity.Y *= gravity;
            }
        }
    }
}