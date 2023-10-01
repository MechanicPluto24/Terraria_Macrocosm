using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	public class SeleniteArrow : ModProjectile, IBullet
	{
		public override void SetDefaults()
		{
			AIType = ProjectileID.Bullet;
			Projectile.width = 14;
			Projectile.height = 14;
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

			for(int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SeleniteBrightDust>(), Scale: Main.rand.NextFloat(0.8f, 1f));
				dust.noGravity = true;
			}
			
			Lighting.AddLight(Projectile.Center, new Color(255, 255, 255).ToVector3() * 0.6f);

			return true;
		}

		public override void OnKill(int timeLeft)
		{
			for(int i = 0; i < Main.rand.Next(20, 30); i++)
				Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity * 0.5f, ModContent.DustType<SeleniteBits>(), Projectile.oldVelocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.2f), Scale: 2f);
		}

		private SpriteBatchState state;
		public override bool PreDraw(ref Color lightColor)
		{
			float count = (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 10f;

			if (count > 40f)
				count = 40f;

			state = Main.spriteBatch.SaveState();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			for (int n = 4; n < count; n++)
			{
				Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 0.2f;
				Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.White * (0.6f - (float)n/count), Projectile.rotation, TextureAssets.Projectile[Type].Value.Size()/2f, Projectile.scale, SpriteEffects.None, 0f);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return true;
		}

		public override void PostDraw(Color lightColor)
		{
		}

		public override Color? GetAlpha(Color lightColor)
			=> new Color(255, 255, 255, 200);
	}
}
