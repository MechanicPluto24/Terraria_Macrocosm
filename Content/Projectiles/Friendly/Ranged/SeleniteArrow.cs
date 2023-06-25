using Terraria.GameContent;
using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Projectiles.Global;
using Macrocosm.Content.Dusts;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	public class SeleniteArrow : ModProjectile, IBullet
	{
		public override void SetDefaults()
		{
			AIType = ProjectileID.Bullet;
			Projectile.width = 16;
			Projectile.height = 16;
 			Projectile.timeLeft = 270;
			Projectile.light = 0f;
			Projectile.friendly = true;
		}
		public override bool PreAI()
		{
			if (Projectile.velocity.X < 0f)
			{
				Projectile.spriteDirection = -1;
				Projectile.rotation = (float)Math.Atan2(0f - Projectile.velocity.Y, 0f - Projectile.velocity.X) - 1.57f;
			}
			else
			{
				Projectile.spriteDirection = 1;
				Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
			}

			Lighting.AddLight(Projectile.Center, new Color(255, 255, 255).ToVector3() * 0.6f);

			for (int i = 0; i < 2; i++)
			{
				Particle.CreateParticle<SeleniteSpark>(Projectile.position + new Vector2(0, 26).RotatedBy(Projectile.rotation), Projectile.velocity,  rotation: Projectile.velocity.X, scale: Main.rand.NextFloat(0.1f, 0.2f));
			}

			return true;
		}

		public override void Kill(int timeLeft)
		{
			for(int i = 0; i < Main.rand.Next(10, 20); i++)
				Particle.CreateParticle<SeleniteSpark>(Projectile.position + Projectile.oldVelocity, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.12f, Scale: 0.3f);
		}

		public override Color? GetAlpha(Color lightColor)
			=> new Color(255, 255, 255, 50);
	}
}
