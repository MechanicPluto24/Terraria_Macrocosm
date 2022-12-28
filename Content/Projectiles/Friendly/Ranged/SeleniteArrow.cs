using Terraria.GameContent;
using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Global.GlobalProjectiles;
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
				Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(0, 26).RotatedBy(Projectile.rotation), 12, 12, ModContent.DustType<SeleniteSparkDust>(), Projectile.velocity.X);
				dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity;
			}

			return true;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SeleniteSparkDust>(), Projectile.velocity.X);
				dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
				dust.noLight = false;
				dust.noGravity = true;
			}
		}

		public override Color? GetAlpha(Color lightColor)
			=> new Color(255, 255, 255, 50);
	}
}
