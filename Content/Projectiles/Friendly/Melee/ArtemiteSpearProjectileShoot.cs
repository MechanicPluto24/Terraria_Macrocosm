using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
	public class ArtemiteSpearProjectileShoot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;

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
				for (int i = 0; i < Main.rand.Next(30, 40); i++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>(), 0f, .1f, Scale: 1f);
					dust.velocity.Y *= 0.02f;
				}
			}
		}
	}
}
