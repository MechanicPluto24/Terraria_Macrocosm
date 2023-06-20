using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Macrocosm.Content.Projectiles.Global;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	public class NWAMissile : ModProjectile, IExplosive
	{
		public ref float AI_HomingTimer => ref Projectile.ai[0];
		public ref float AI_AccelerationTimer => ref Projectile.ai[1];

		public float BlastRadius => 100;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 30;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 38;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.light = .5f;

			Projectile.SetTrail<MissileTrail>();
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			// FIXME: left rotation 
			float rotation = Math.Abs(Projectile.rotation);
			if (rotation < 0 && rotation <= MathHelper.PiOver4 && rotation >= MathHelper.PiOver4 * 3 || rotation >= 0 && rotation > MathHelper.PiOver4 && rotation <= MathHelper.PiOver4 * 3)
				hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.height, Projectile.width);
			else
				hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
		}

		/// <summary> Adapted from Projectile.AI_016() for homing snowman rockets </summary>
		public override void AI()
		{
			#region Acceleration

			float timeToReachTopSpeed = 100;
			if(AI_AccelerationTimer < timeToReachTopSpeed)
			{
				AI_AccelerationTimer++;
			}

			#endregion

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

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					// only run locally
					if (Projectile.owner == Main.myPlayer && Main.npc[i].CanBeChasedBy(this) && Main.npc[i].Macrocosm().TargetedByHomingProjectile)
					{
						float targetCenterX = Main.npc[i].position.X + Main.npc[i].width / 2;
						float targetCenterY = Main.npc[i].position.Y + Main.npc[i].height / 2;
						float distanceL1 = Math.Abs(Projectile.position.X + Projectile.width / 2 - targetCenterX) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - targetCenterY);
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
				x = Projectile.position.X + Projectile.width / 2 + Projectile.velocity.X * 100f;
				y = Projectile.position.Y + Projectile.height / 2 + Projectile.velocity.Y * 100f;
			}
			else
			{
				// targeting was done locally, sync
				Projectile.netUpdate = true; 
			}

			Vector2 maxVelocity = (new Vector2(x, y) - Projectile.Center).SafeNormalize(-Vector2.UnitY) * (10f + 6f * (AI_AccelerationTimer / timeToReachTopSpeed));
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

			// alpha fade-in
			Projectile.localAI[1] += 0.6f;
			if (Projectile.localAI[1] > 6f)
			{
				Projectile.localAI[1] = 6f;
				Projectile.alpha = 0;
			}
			else
			{
				Projectile.alpha = (int)(255f - 42f * Projectile.localAI[1]) + 100;
				if (Projectile.alpha > 255)
					Projectile.alpha = 255;

				// spawn some dusts as barrel flash
				for (int i = 0; i < 10; i++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(0, 0), Projectile.width, Projectile.height, DustID.Torch, Scale: 3);
					dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(6f, 12f)).RotatedByRandom(MathHelper.PiOver4 * 0.4);
					dust.noLight = false;
					dust.alpha = 200;
					dust.noGravity = true;
				}
			}

			// spawn dust trail
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
					Main.dust[flameIdx].scale *= 1.4f + Main.rand.Next(10) * 0.1f;
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
					Main.dust[smokeIdx].fadeIn = 0.5f + Main.rand.Next(5) * 0.1f;
					Main.dust[smokeIdx].velocity *= 0.05f;
				}
			}

			#endregion
		}

		public override void Kill(int timeLeft)
		{
			if (Main.dedServ)
				return;
 
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

			#region Spawn dusts
			for (int i = 0; i < 30; i++)
			{
				int dustSmoke = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
				Dust dust = Main.dust[dustSmoke];
				dust.velocity *= 1.4f;
			}

			for (int i = 0; i < 20; i++)
			{
				int dustFlame = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
				Main.dust[dustFlame].noGravity = true;
				Dust dust = Main.dust[dustFlame];
				dust.velocity *= 7f;
				dustFlame = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
				dust = Main.dust[dustFlame];
				dust.velocity *= 3f;
			}
			#endregion

			#region Spawn trail dust
			for (int i = 0; i < Projectile.oldPos.Length; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Vector2 pos = Projectile.oldPos[i];
					Dust dust = Dust.NewDustDirect(new Vector2(pos.X, pos.Y), 20, 20, DustID.Torch, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, Scale: 0.07f * (Projectile.oldPos.Length - i));
 				}
			}
			#endregion

			#region Spawn Smoke Gores
			for (int i = 0; i < 2; i++)
			{
				float multVelocity = 0.4f;
				if (i == 1)
					multVelocity = 0.8f;

				int goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
				Gore gore = Main.gore[goreSmoke];
				gore.velocity *= multVelocity;
				Main.gore[goreSmoke].velocity.X += 1f;
				Main.gore[goreSmoke].velocity.Y += 1f;
				goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
				gore = Main.gore[goreSmoke];
				gore.velocity *= multVelocity;
				Main.gore[goreSmoke].velocity.X -= 1f;
				Main.gore[goreSmoke].velocity.Y += 1f;
				goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
				gore = Main.gore[goreSmoke];
				gore.velocity *= multVelocity;
				Main.gore[goreSmoke].velocity.X += 1f;
				Main.gore[goreSmoke].velocity.Y -= 1f;
				goreSmoke = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3));
				gore = Main.gore[goreSmoke];
				gore.velocity *= multVelocity;
				Main.gore[goreSmoke].velocity.X -= 1f;
				Main.gore[goreSmoke].velocity.Y -= 1f;
			}
			#endregion
		 
		}

		public override bool PreDraw(ref Color lightColor)
		{
			// adjust rotations manually (due to TrailingMode = 0 instead of 3) -- it just looks better  
			if (Projectile.rotation > -MathHelper.PiOver4 && Projectile.rotation < MathHelper.PiOver4 || 
				Projectile.rotation < -(MathHelper.PiOver4 + MathHelper.PiOver2) ||
				Projectile.rotation > MathHelper.PiOver4 + MathHelper.PiOver2) {

				for (int i = 0; i < Projectile.oldRot.Length; i++)
					Projectile.oldRot[i] = MathHelper.PiOver2;
			}

			Projectile.GetTrail().Opacity = Projectile.localAI[1];

			if (Projectile.alpha < 1)
				Projectile.GetTrail().Draw();

			return true;
		}
	}
}