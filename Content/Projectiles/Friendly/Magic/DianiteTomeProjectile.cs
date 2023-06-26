using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Macrocosm.Content.Trails;
using Terraria.DataStructures;
using static tModPorter.ProgressUpdate;
using static Terraria.ModLoader.PlayerDrawLayer;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	public class DianiteTomeProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 18;
			ProjectileID.Sets.TrailingMode[Type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.aiStyle = 56;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;

			Projectile.SetTrail<DianiteMeteorTrail>();
		}

		public ref float InitialTargetPositionY => ref Projectile.ai[0];


		bool spawned = false;
		bool rotationClockwise = false;

		public override void AI()
		{
			if (!spawned)
			{
				rotationClockwise = Main.rand.NextBool();
				spawned = true;

				// sync ai array on spawn
				Projectile.netUpdate = true;
			}

			if (Main.rand.NextBool())
			{
				Vector2 velocity = -Projectile.velocity.RotatedByRandom(MathHelper.Pi / 2f) * 0.1f;
				Dust dust = Dust.NewDustDirect(Projectile.position, (int)(Projectile.width), 1, DustID.Flare, velocity.X, velocity.Y, Scale: 1f);
				dust.noGravity = true;
			}
		
			if (rotationClockwise)
				Projectile.rotation += 0.2f;
			else
				Projectile.rotation -= 0.2f;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			if(InitialTargetPositionY > Projectile.position.Y)
				return false;

			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			float count = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) * 10f;

			if (count > 50f)
				count = 50f;

			var state = Main.spriteBatch.SaveState();

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			Projectile.GetTrail().Draw();
			for (int n = 2; n < count; n++)
			{
				Vector2 trailPosition = Projectile.Center - Projectile.velocity * n * 0.15f;
				Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.OrangeRed * (0.75f - (float)n / count), Projectile.rotation + ((float)n/count), TextureAssets.Projectile[Type].Value.Size() / 2f, Projectile.scale * (1f - (float)n / count), SpriteEffects.None, 0f);
			}


			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return true;
		}

		public override void Kill(int timeLeft)
		{

			for(int i = 0; i < 3; i++)
			{
				Gore smoke = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64), Scale: 0.6f);
				smoke.velocity *= 0.3f;
				smoke.velocity.X += Main.rand.Next(-10, 11) * 0.15f;
				smoke.velocity.Y += Main.rand.Next(-10, 11) * 0.15f;

				
			}

			for (int i = 0; i < 15; i++)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(25f, 25f);
				Dust dust = Dust.NewDustDirect(Projectile.position, (int)(Projectile.width), Projectile.height, DustID.Flare, velocity.X, velocity.Y, Scale: 3f);
				dust.noGravity = true;
			}

			Particle big = Particle.CreateParticle<TintableExplosion>(p =>
			{
				p.Position = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
				p.DrawColor = new Color(255, 104, 9) * Main.rand.NextFloat(0.8f, 0.9f);
				p.Scale = 1.1f;
			});

			Particle small = Particle.CreateParticle<TintableExplosion>(p =>
			{
				p.Position = Projectile.Center + Main.rand.NextVector2Circular(5f, 5f);
				p.DrawColor = new Color(255, 104, 9) * Main.rand.NextFloat(0.8f, 0.9f);
				p.Scale = 0.9f;
			});
		}
	}
}