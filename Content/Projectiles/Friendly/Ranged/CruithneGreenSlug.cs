using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
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
			Projectile.timeLeft = 270;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Point coordinates = (Projectile.Center * oldVelocity).ToTileCoordinates();
			WorldGen.KillTile(coordinates.X, coordinates.Y + 1, effectOnly: true);
			return true;
		}
	}
}
