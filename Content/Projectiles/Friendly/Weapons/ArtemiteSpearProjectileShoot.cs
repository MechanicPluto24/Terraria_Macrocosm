using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Weapons
{
	public class ArtemiteSpearProjectileShoot : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Artemite Spear"); 
		}

		public override void SetDefaults() {
			Projectile.width = 18; 
			Projectile.height = 38 - 8; 

			AIType = ProjectileID.WoodenArrowFriendly;

			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.DamageType = DamageClass.Melee; 
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true; // Can the projectile collide with tiles?

			Projectile.penetrate = 3; // 3 max hits 
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.velocity.Y += 0.03f;

			// Avoid spawning dusts on dedicated servers
			if (!Main.dedServ)
			{
				if (Main.rand.NextBool(2))
				{
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>(), Projectile.velocity.X / -64f, Projectile.velocity.Y / -16f, Scale: 0.8f);
				}
			}
		}

        public override void Kill(int timeLeft)
        {
            if (!Main.dedServ)
            {
				for (int i = 0; i < Main.rand.Next(10, 20); i++)
				{
					int dustIdx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>(), 0f, .1f, Scale: 1f);
					Main.dust[dustIdx].velocity.Y *= 0.2f;
				}
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
			// 3a: target.immune[Projectile.owner] = 20;
			// 3b: target.immune[Projectile.owner] = 5;
		}
	}
}
