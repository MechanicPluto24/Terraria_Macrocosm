using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	public class DianiteTomeProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dianite Meteor");
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = 56;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
		}
	}
}