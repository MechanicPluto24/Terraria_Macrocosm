using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
	public class PhantasmalImpSmall : ModProjectile
	{
		public Player TargetPlayer => Main.player[(int)Projectile.ai[2]];

		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 54;
			Projectile.hostile = true;
			Projectile.timeLeft = 5 * 60;
		}

		bool spawned = false;
		public override void AI()
		{
			Vector2 direction = (TargetPlayer.Center - Projectile.Center).SafeNormalize(Vector2.One);

			Projectile.velocity += direction * 0.1f;

			float maxOffset = MathHelper.ToRadians(3); 
			float randomOffset = Main.rand.NextFloat(-maxOffset, maxOffset);
			Projectile.velocity = Projectile.velocity.RotatedBy(randomOffset);

			Projectile.direction = System.Math.Sign(Projectile.velocity.X);
			Projectile.spriteDirection = Projectile.direction;
			Projectile.rotation = Projectile.velocity.X * 0.05f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			return true;
		}
	}
}
