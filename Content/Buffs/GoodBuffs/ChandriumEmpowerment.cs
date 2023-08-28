using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Macrocosm.Content.Rockets;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.GoodBuffs
{
    internal class ChandriumEmpowerment : ModBuff
	{
		public override void SetStaticDefaults()
		{
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.buffTime[buffIndex] <= 1)
			{
				player.Macrocosm().ChandriumEmpowermentStacks = 0;
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
					particle.Kill();
			}
		}
	}
}
