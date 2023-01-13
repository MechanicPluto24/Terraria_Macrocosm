using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	public class DianiteTomeProjectileSmall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dianite Meteor");
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 56;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			return true;
		}
	}
}