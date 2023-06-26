using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	public class DianiteTomeProjectileSmall : DianiteTomeProjectile
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 16;
			Projectile.height = 16;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
			float count = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) * 10f;

			if (count > 25f)
				count = 25f;

			var state = Main.spriteBatch.SaveState();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			for (int n = 2; n < count - 5; n++)
			{
				Vector2 trailPosition = Projectile.Center - Projectile.velocity * n * 0.15f;
				Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.OrangeRed * (0.8f - (float)n / count), Projectile.rotation + ((float)n / count), TextureAssets.Projectile[Type].Value.Size() / 2f, Projectile.scale * (1f - (float)n / count), SpriteEffects.None, 0f);
			}

			Projectile.GetTrail().Draw();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return true;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f);
				Dust dust = Dust.NewDustDirect(Projectile.position, (int)(Projectile.width), Projectile.height, DustID.Flare, velocity.X, velocity.Y, Scale: 2.2f);
				dust.noGravity = true;
			}

			for (int i = 0; i < 2; i++)
			{
				Gore smoke = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64), Scale: 0.4f);
				smoke.velocity *= 0.3f;
				smoke.velocity.X += Main.rand.Next(-10, 11) * 0.15f;
				smoke.velocity.Y += Main.rand.Next(-10, 11) * 0.15f;
			}

			Particle explosion = Particle.CreateParticle<TintableExplosion>(p =>
			{
				p.Position = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
				p.DrawColor = new Color(255, 104, 9) * Main.rand.NextFloat(0.8f, 0.9f);
				p.Scale = 0.8f;
				p.NumberOfInnerReplicas = 2;
				p.ReplicaScalingFactor = 0.2f;
			});
		}
	}
}