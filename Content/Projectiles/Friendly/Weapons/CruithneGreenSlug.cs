using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Weapons
{
	public class CruithneGreenSlug : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(14);
			AIType = ProjectileID.Bullet;
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.extraUpdates = 3;
			Projectile.timeLeft = 180;
		}
	}
}
