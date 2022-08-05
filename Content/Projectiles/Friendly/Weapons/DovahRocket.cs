using Macrocosm.NPCs.GlobalNPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Weapons
{
	public class DovahRocket : ModProjectile
	{

		public ref float AI_HomingTimer => ref Projectile.ai[0];

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rocket");
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 38;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.light = .5f;
		}

		/// <summary>
		/// Adapted from Projectile.AI_016() for homing snowman rockets
		/// </summary>
		public override void AI()
		{

			#region Homing

			float x = Projectile.position.X;
			float y = Projectile.position.Y;

			float maxDistance = 800f; // original was 600f
			float minHomingTime = 20f; // original was 30f

			bool hasTarget = false;
			AI_HomingTimer++;
			if (AI_HomingTimer > minHomingTime)
			{
				AI_HomingTimer = minHomingTime;

				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].CanBeChasedBy(this) && Main.npc[i].GetGlobalNPC<InstancedGlobalNPC>().targetedBy[Projectile.owner])
					{
						float targetCenterX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
						float targetCenterY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
						float distanceL1 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - targetCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - targetCenterY);
						if (distanceL1 < maxDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
						{
							maxDistance = distanceL1;
							x = targetCenterX;
							y = targetCenterY;
							hasTarget = true;
						}
					}
				}
			}

			if (!hasTarget)
			{
				x = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
				y = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
			}

			Vector2 maxVelocity = (new Vector2(x, y) - Projectile.Center).SafeNormalize(-Vector2.UnitY) * 16f;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, maxVelocity, 0.083333336f);

			#endregion

			#region Rotation

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

			#endregion

			#region Visual Effects

			Projectile.localAI[1] += 1f;
			if (Projectile.localAI[1] > 6f)
			{
				Projectile.alpha = 0;
			}
			else
			{
				Projectile.alpha = (int)(255f - 42f * Projectile.localAI[1]) + 100;
				if (Projectile.alpha > 255)
					Projectile.alpha = 255;
			}

			for (int i = 0; i < 2; i++)
			{

				Vector2 dustVelocity = Vector2.Zero;
				if (i == 1)
					dustVelocity = Projectile.velocity * 0.5f;


				if (!(Projectile.localAI[1] > 9f))
					continue;

				if (Main.rand.NextBool(2))
				{
					int flameIdx = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dustVelocity.X, Projectile.position.Y + 3f + dustVelocity.Y) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Torch, 0f, 0f, 100);
					Main.dust[flameIdx].scale *= 1.4f + (float)Main.rand.Next(10) * 0.1f;
					Main.dust[flameIdx].velocity *= 0.2f;
					Main.dust[flameIdx].noGravity = true;
					if (Main.dust[flameIdx].type == 152)
					{
						Main.dust[flameIdx].scale *= 0.5f;
						Main.dust[flameIdx].velocity += Projectile.velocity * 0.1f;
					}
					else if (Main.dust[flameIdx].type == 35)
					{
						Main.dust[flameIdx].scale *= 0.5f;
						Main.dust[flameIdx].velocity += Projectile.velocity * 0.1f;
					}
					else if (Main.dust[flameIdx].type == Dust.dustWater())
					{
						Main.dust[flameIdx].scale *= 0.65f;
						Main.dust[flameIdx].velocity += Projectile.velocity * 0.1f;
					}
				}

				if (Main.rand.NextBool(2))
				{
					int smokeIdx = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dustVelocity.X, Projectile.position.Y + 3f + dustVelocity.Y) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
					Main.dust[smokeIdx].fadeIn = 0.5f + (float)Main.rand.Next(5) * 0.1f;
					Main.dust[smokeIdx].velocity *= 0.05f;
				}
			}

			#endregion
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{

			Projectile.tileCollide = false;
			Projectile.timeLeft = 3;
			Explode();
			return true;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.timeLeft = 0;
			Explode();
		}

		public void Explode()
		{
			Projectile.velocity = Vector2.Zero;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.knockBack = 12f;

			Projectile.position.X += Projectile.width / 2;
			Projectile.position.Y += Projectile.height / 2;
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.position.X -= Projectile.width / 2;
			Projectile.position.Y -= Projectile.height / 2;
		}

		public override void Kill(int timeLeft)
		{

			if (Main.netMode != NetmodeID.Server)
			{

				SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

				for (int i = 0; i < 30; i++)
				{
					int dustSmoke = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
					Dust dust = Main.dust[dustSmoke];
					dust.velocity *= 1.4f;
				}

				for (int i = 0; i < 20; i++)
				{
					int dustFlame = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
					Main.dust[dustFlame].noGravity = true;
					Dust dust = Main.dust[dustFlame];
					dust.velocity *= 7f;
					dustFlame = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
					dust = Main.dust[dustFlame];
					dust.velocity *= 3f;
				}

				for (int i = 0; i < 2; i++)
				{
					float multVelocity = 0.4f;
					if (i == 1)
						multVelocity = 0.8f;

					int goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
					Gore gore = Main.gore[goreSmoke];
					gore.velocity *= multVelocity;
					Main.gore[goreSmoke].velocity.X += 1f;
					Main.gore[goreSmoke].velocity.Y += 1f;
					goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
					gore = Main.gore[goreSmoke];
					gore.velocity *= multVelocity;
					Main.gore[goreSmoke].velocity.X -= 1f;
					Main.gore[goreSmoke].velocity.Y += 1f;
					goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
					gore = Main.gore[goreSmoke];
					gore.velocity *= multVelocity;
					Main.gore[goreSmoke].velocity.X += 1f;
					Main.gore[goreSmoke].velocity.Y -= 1f;
					goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
					gore = Main.gore[goreSmoke];
					gore.velocity *= multVelocity;
					Main.gore[goreSmoke].velocity.X -= 1f;
					Main.gore[goreSmoke].velocity.Y -= 1f;
				}
			}
		}
	}
}