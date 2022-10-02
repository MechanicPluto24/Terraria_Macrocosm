using Macrocosm.Common.Utility;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Gores;
using Macrocosm.Content.Items.Miscellaneous;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Meteors
{
	public abstract class BaseMeteor : ModProjectile
	{
		public int Width;
		public int Height;
		public int Damage;

		public float ScreenshakeMaxDist;
		public float ScreenshakeIntensity;

		public float RotationMultiplier;
		public float BlastRadiusMultiplier = 1f;

		public int DustType = -1;
		public int ImpactDustCount;
		public Vector2 ImpactDustSpeed;
		public float ImpactDustScaleMin;
		public float ImpactDustScaleMax;
		public int AiDustChanceDenominator;

		public int GoreType = -1;
		public int GoreCount;
		public Vector2 GoreVelocity;

		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;

			Projectile.width = Width;
			Projectile.height = Height;
			Projectile.damage = Damage;
		}

		public override void Kill(int timeLeft)
		{
			// handled by clients 
			if (Main.netMode != NetmodeID.Server)
			{
				SpawnImpactDusts();
				SpawnGores();
				ImpactSounds();
			}

			// handled by server 
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				SpawnItems();
				ImpactScreenshake();
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.Explode(Width * BlastRadiusMultiplier);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Explode(Width * BlastRadiusMultiplier);
			return false;
		}

		override public void AI()
		{
			AIRotation();
			AIDusts();
			ExtraAI();


		}

		public virtual void SpawnImpactDusts()
		{
			if (DustType < 0)
				return;

			for (int i = 0; i < ImpactDustCount; i++)
			{
				Dust dust = Dust.NewDustDirect(
					new Vector2(Projectile.Center.X, Projectile.Center.Y + 0.25f * Projectile.height),
					Width,
					Height,
					DustType,
					Main.rand.NextFloat(-ImpactDustSpeed.X, ImpactDustSpeed.X),
					Main.rand.NextFloat(0f, -ImpactDustSpeed.Y),
					Scale: Main.rand.NextFloat(ImpactDustScaleMin, ImpactDustScaleMax)
				);

				dust.noGravity = false;
			}

		}

		public virtual void SpawnGores()
		{
			if (GoreType < 0)
				return;

			for (int i = 0; i < GoreCount; i++)
			{
				Gore.NewGore(Projectile.GetSource_FromThis(),
					new Vector2(Projectile.Center.X, Projectile.Center.Y ),
					new Vector2(Projectile.velocity.X * GoreVelocity.X, -Projectile.velocity.Y * GoreVelocity.Y) * Main.rand.NextFloat(0.5f, 1f),
					GoreType);
			}
		}

		public virtual void ImpactSounds() { }
		public virtual void SpawnItems() { }

		public virtual void ImpactScreenshake()
		{
			for (int i = 0; i < 255; i++)
			{
				Player player = Main.player[i];
				if (player.active)
				{
					float distance = Vector2.Distance(player.Center, Projectile.Center);
					if (distance < ScreenshakeMaxDist)
					{
						player.SetScreenshake(ScreenshakeIntensity - distance / ScreenshakeMaxDist * ScreenshakeIntensity);
					}
				}
			}
		}

		public virtual void AIRotation()
		{
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * RotationMultiplier * Projectile.direction;

		}

		public virtual void AIDusts()
		{
			if (DustType == -1)
				return;

			if (Main.rand.NextBool(AiDustChanceDenominator))
			{
				Dust dust = Dust.NewDustDirect(
						new Vector2(Projectile.position.X, Projectile.position.Y),
						Projectile.width,
						Projectile.height,
						DustType,
						0f,
						0f,
						Scale: Main.rand.NextFloat(ImpactDustScaleMin, ImpactDustScaleMax)
					);

				dust.noGravity = true;
			}
		}

		public virtual void ExtraAI() { }
		
	}
}
