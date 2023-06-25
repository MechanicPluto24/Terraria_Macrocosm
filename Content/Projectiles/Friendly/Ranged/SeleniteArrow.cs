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
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	public class SeleniteArrow : ModProjectile, IBullet
	{
		public override void SetDefaults()
		{
			AIType = ProjectileID.Bullet;
			Projectile.width = 4;
			Projectile.height = 4;
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

			return true;
		}

		public override void Kill(int timeLeft)
		{
			for(int i = 0; i < Main.rand.Next(20, 30); i++)
				Particle.CreateParticle<SeleniteSpark>(Projectile.Center + Projectile.oldVelocity * 0.4f, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.3f), scale: 2f);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			float count = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) * 10f;

			if (count > 6f)
				count = 6f;

			var state = Main.spriteBatch.SaveState();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			for (int n = 0; n < count; n++)
			{
				Vector2 trailPosition = Projectile.Center - Projectile.velocity * n * 0.3f;
				Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.White * (1f - (float)n/count), Projectile.rotation, new Vector2(4f, 4f), Projectile.scale, SpriteEffects.None, 0f);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return true;
		}

		public override void PostDraw(Color lightColor)
		{
		}

		public override Color? GetAlpha(Color lightColor)
			=> new Color(255, 255, 255, 255);
	}
}
