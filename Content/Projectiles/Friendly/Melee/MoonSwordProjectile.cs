using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
	public class MoonSwordProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 6;
			ProjectileID.Sets.TrailCacheLength[Type] = 20;
			ProjectileID.Sets.TrailingMode[Type] = 3;
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
			Projectile.penetrate = 5;
		}

		// PreDraw
		public override bool PreDraw(ref Color lightColor)
		{
			return true;
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