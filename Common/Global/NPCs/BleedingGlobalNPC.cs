using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.NPCs
{
    public class BleedingGlobalNPC : GlobalNPC
    {
        public override void AI(NPC npc)
        {
            if (npc.HasBuff(BuffID.Bleeding))
            {
                if (Main.rand.NextBool(30) && npc.Opacity > 0.1f)
                {
                    npc.HitEffect();
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff(BuffID.Bleeding))
                npc.lifeRegen -= 10;
        }
    }
}
