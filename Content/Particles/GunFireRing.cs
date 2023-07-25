using Terraria;
using Macrocosm.Common.Drawing.Particles;
using Terraria.ModLoader;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Content.Projectiles.Friendly.Ranged;

namespace Macrocosm.Content.Particles
{
	public class GunFireRing : Particle
	{
		public override int FrameNumber => 4;
		public override int FrameSpeed => 6;
		public override bool DespawnOnAnimationComplete => true;

		public override void OnSpawn()
		{
  		}

		public override void AI()
		{
 		}
		
		public override void OnKill()
		{
		}
	}
}
