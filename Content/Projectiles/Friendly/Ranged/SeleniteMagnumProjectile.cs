using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Base;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Terraria.DataStructures;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class SeleniteMagnumProjectile : RicochetBullet
	{
		public override int RicochetCount => 2;

		public override float RicochetSpeed => 15f;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 4;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetProjectileDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.timeLeft = 600;
		}

		public override void OnRicochet()
		{
			Projectile.damage -= 5;
		}

		public override void OnSpawn(IEntitySource source)
		{
			//for (int i = 0; i < Main.rand.Next(6, 12); i++)
			//	Dust.NewDustPerfect(Projectile.position,ModContent.DustType<SeleniteSparkDust>(), Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.14f, 0.18f), Scale: 0.3f);
			//Dust.NewDustPerfect(Projectile.position,ModContent.DustType<SeleniteSparkDust>(), Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(0.1f, 0.14f), Scale: 0.5f);
		}

		public override void OnRicochetEffect()
		{
			for (int i = 0; i < Main.rand.Next(10, 20); i++)
				Dust.NewDustPerfect(Projectile.position, ModContent.DustType<SeleniteSparkDust>(), Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.1f, Scale: 0.3f);
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for(int i = 0; i < Main.rand.Next(10, 20); i++)
				Dust.NewDustPerfect(Projectile.position + Projectile.oldVelocity, ModContent.DustType<SeleniteSparkDust>(), Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.12f, Scale: 0.3f);

			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			//	Projectile.DrawSimpleTrail(Vector2.Zero, 2f, 0.5f, new Color(158, 202, 206) * lightColor.GetLuminance(), new Color(255, 255, 255, 0) * lightColor.GetLuminance());
			return true;
		}
	}
}
