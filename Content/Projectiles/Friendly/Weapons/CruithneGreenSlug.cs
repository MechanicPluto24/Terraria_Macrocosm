using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Weapons
{
	public class CruithneGreenSlug : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.CloneDefaults(14);
			aiType = ProjectileID.Bullet;
			projectile.width = 4;
			projectile.height = 4;
			projectile.extraUpdates = 3;
			projectile.timeLeft = 180;
		}
	}
}
