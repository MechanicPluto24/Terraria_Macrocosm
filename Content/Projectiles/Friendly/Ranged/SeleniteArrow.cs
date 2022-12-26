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

			for (int i = 0; i < 1; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(0, 26).RotatedBy(Projectile.rotation), 12, 12, ModContent.DustType<LuminiteSparkDust>(), Projectile.velocity.X);
				dust.scale = Main.rand.NextFloat(0.6f, 1f);
				dust.noGravity = true;
				dust.velocity = Projectile.velocity;
			}

			return true;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteSparkDust>(), Projectile.velocity.X);
				dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
				dust.noLight = false;
				dust.noGravity = true;
			}
		}

		SpriteBatchState state;
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[Type].Value;
			//state = Main.spriteBatch.SaveState();
			//Main.spriteBatch.End();
			//Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(51, 185, 131, 50), Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
		public override void PostDraw(Color lightColor)
		{
			//Main.spriteBatch.Restore(state);
		}
	}
}
