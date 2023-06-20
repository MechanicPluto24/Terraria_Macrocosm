using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class PhantasmalImpLarge : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.hostile = true;
		}


		bool spawned = false;
		public override bool PreAI()
		{
			Projectile.direction = System.Math.Sign(Projectile.velocity.X);
			Projectile.spriteDirection = Projectile.direction;
			Projectile.rotation = Projectile.velocity.X * 0.05f;
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			return true;
		}
	}
}
