using Macrocosm.Common.Drawing.Particles;

namespace Macrocosm.Content.Particles
{
	internal class GunFireRing : Particle
	{
		public override int FrameNumber => 4;
		public override int FrameSpeed => 8;
		public override bool DespawnOnAnimationComplete => true;

		public override void OnSpawn()
		{
  		}

		public override void AI()
		{
			Velocity *= 0.94f;
 		}
		
		public override void OnKill()
		{
		}
	}
}
