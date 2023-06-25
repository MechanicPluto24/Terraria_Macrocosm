using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Base;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Terraria.DataStructures;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class SeleniteMagnumProjectile : RicochetBullet
	{
		public override int RicochetCount => 2;

		public override float RicochetSpeed => 15f;

		public override void SetStaticDefaults()
		{
		}

		public override void SetProjectileDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.timeLeft = 600;
		}

		public override void OnRicochet()
		{
			Projectile.damage -= 5;
		}

		public override void OnRicochetEffect()
		{
			for (int i = 0; i < Main.rand.Next(10, 20); i++)
				Particle.CreateParticle<SeleniteSpark>(Projectile.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.1f, Scale: 0.3f)
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for(int i = 0; i < Main.rand.Next(10, 20); i++)
				Particle.CreateParticle<SeleniteSpark>(Projectile.position + Projectile.oldVelocity, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.12f, Scale: 0.3f);

			return true;
		}
	}
}
