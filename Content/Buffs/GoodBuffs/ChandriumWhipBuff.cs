using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Players;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.GoodBuffs
{
    public class ChandriumWhipBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] <= 1)
            {
                player.GetModPlayer<MacrocosmPlayer>().ChandriumWhipStacks = 0;
                KillParticle();
            }

        }

        public override bool RightClick(int buffIndex)
        {
            KillParticle();
            return true;
        }

        public static void KillParticle()
        {
            foreach (Particle particle in ParticleManager.Particles)
            {
                if (particle is ChandriumSparkle chandriumSparkle && chandriumSparkle.Owner == Main.myPlayer)
                {
                    particle.Kill(shouldSync: true);
                }
            }
        }
    }
}
