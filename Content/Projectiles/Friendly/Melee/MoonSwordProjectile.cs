using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
	public class MoonSwordProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 6;
			DisplayName.SetDefault("Moonlight Greatsword");
		}

		public override void SetDefaults()
		{
			Projectile.width = 62;
			Projectile.height = 98;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
 			int frameSpeed = 3;
			if (Projectile.frameCounter++ >= frameSpeed)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;

				if (Projectile.frame >= Main.projFrames[Type])
 					Projectile.frame = 0;
 			}
 
		}

	
	}
}