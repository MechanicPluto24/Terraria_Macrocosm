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
                    Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Blood);
                    dust.velocity.Y += 0.5f;
                    dust.velocity *= 0.25f;
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
