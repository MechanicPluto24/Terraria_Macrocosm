using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Global.GlobalProjectiles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	public class PlasmaGrenade : ModProjectile
	{
		public bool Exploded
		{
			get => Projectile.ai[0] != 0f;
			set => Projectile.ai[0] = value ? 1f : 0f;
		}

		public int PlasmaBallCount
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}
		
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 40;
			ProjectileID.Sets.TrailingMode[Type] = 2;
			ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Ranged;

			Projectile.width = 16;
			Projectile.height = 16;

			Projectile.timeLeft = 180;

			Projectile.tileCollide = true;

			Projectile.friendly = true;
			Projectile.hostile = false;
 			Projectile.penetrate = -1;
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;

			Projectile.aiStyle = -1;
		}

		public override void OnSpawn(IEntitySource source)
		{
			PlasmaBallCount = 300;
		}

		public override bool PreAI()
		{
			return true;
		}

		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.407f, 1f, 1f);

			if (Exploded)
				return;
 
			float gravity = 0.2f * MacrocosmSubworld.CurrentGravityMultiplier;

			if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
			{
				Projectile.velocity.X *= 0.97f;
				if (Projectile.velocity.X is > -0.01f and < 0.01f)
				{
					Projectile.velocity.X = 0f;
					Projectile.netUpdate = true;
				}
			}

			Projectile.velocity.Y += gravity;
			Projectile.rotation += Projectile.velocity.X * 0.1f;

			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.PlasmaBallDust>(), Scale: Main.rand.NextFloat(0.8f, 1.2f));
			}

			if (Projectile.timeLeft < 5)
			{
				Explode();
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
 				Projectile.velocity.X = oldVelocity.X * -0.4f;
			 
			if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
 				Projectile.velocity.Y = oldVelocity.Y * -0.4f;

			return false;
		}

		public override bool PreKill(int timeLeft)
		{
			return false;
		}

		public override void Kill(int timeLeft)
		{
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Explode();

			if (Exploded)
				target.AddBuff(BuffID.Slow, 95);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			Explode();
			
			if (Exploded)
				target.AddBuff(BuffID.Slow, 95);
		}

		public void Explode()
		{
			if (Exploded)
				return;

			Projectile.Explode(200, 95);
			
			if (PlasmaBallCount > 0 && Projectile.owner == Main.myPlayer)
			{
				for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / PlasmaBallCount)
				{
					float speed = Main.rand.NextFloat(2f, 15f);
					float theta = i + Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) * 0.5f;
					Vector2 velocity = Utility.PolarVector(speed, theta);

					Particle.CreateParticle<PlasmaBall>(Projectile.Center, velocity, scale: Main.rand.NextFloat(0.2f, 1.2f), shouldSyncOnSpawn: true);
				}
			}

			Exploded = true;
		}
	}
}
