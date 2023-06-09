using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Base;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class ArchersLineProjectile : RicochetBullet
	{
		public override string Texture => "Terraria/Images/Projectile_14";

		public override int RicochetCount => 10;

		public override float RicochetSpeed => 20f;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
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
			Projectile.damage -= 10;
		}

		public override void OnRicochetEffect()
		{

		}

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.DrawSimpleTrail(Vector2.Zero, 2f, 0.5f, new Color(254, 121, 2) * lightColor.GetLuminance(), new Color(184, 58, 24, 0) * lightColor.GetLuminance());
			return true;
		}
	}
}
