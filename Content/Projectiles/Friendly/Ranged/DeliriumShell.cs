using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	public class DeliriumShell : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(14);
			AIType = ProjectileID.Bullet;
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.extraUpdates = 3;
			Projectile.timeLeft = 270;
		}

		bool spawned = false;
		public override bool PreAI()
		{
			if (!spawned)
			{
				// spawn some dusts as "muzzle flash"
				for (int i = 0; i < 20; i++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PortalLightGreenDust>(), Scale: 2.2f);
					dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(3f, 6f)).RotatedByRandom(MathHelper.PiOver4 * 0.4);
					dust.noLight = false;
					dust.alpha = 200;
					dust.noGravity = true;
				}
				spawned = true;
			}

			// spawn dust trail 
			if (Main.rand.NextBool(1))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PortalLightGreenDust>(), Scale: 1.3f);
				dust.noLight = false;
				dust.alpha = 255;
				dust.noGravity = true;
			}

			return true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.Explode(35);
			Projectile.Kill();
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			Projectile.Explode(35);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Explode(35);
			Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
			return true;
		}

		public override void Kill(int timeLeft)
		{
			//spawn dust explosion on kill
			for (int i = 0; i < 60; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PortalLightGreenDust>(), Scale: 2.2f);
				dust.velocity = (Vector2.UnitX * Main.rand.NextFloat(1f, 5f)).RotatedByRandom(MathHelper.TwoPi);
				dust.noLight = false;
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = ModContent.Request<Texture2D>(Texture + "Aura", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			SpriteBatchState state = Main.spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);
			Main.EntitySpriteDraw(tex, Projectile.Center - new Vector2(0, 18).RotatedBy(Projectile.rotation - MathHelper.Pi) - Main.screenPosition, null, Color.White.NewAlpha(255), Projectile.rotation - MathHelper.Pi, tex.Size() / 2, 0.8f, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
			return true;
		}
	}
}
