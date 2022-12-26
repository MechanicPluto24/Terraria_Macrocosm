using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Utility;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.GlobalProjectiles
{
	public class MacrocosmGlobalProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		public Trail Trail { get; set; }

		public override void SetDefaults(Projectile projectile)
		{
			
		}

		public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
		{
			if(projectile.ModProjectile is IBullet)  
				Collision.HitTiles(projectile.position, oldVelocity, projectile.width, projectile.height);

			if (projectile.ModProjectile is IExplosive explosive)
			{
				projectile.Explode(explosive.BlastRadius);
				return false;
			}

			return true;
		}

		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{ 
			if(projectile.ModProjectile is IExplosive explosive)
			{
				projectile.Explode(explosive.BlastRadius);
				projectile.Kill();  
			}
		}

		public override void OnHitPvp(Projectile projectile, Player target, int damage, bool crit)
		{
			if (projectile.ModProjectile is IExplosive explosive)
				projectile.Explode(explosive.BlastRadius); 
		}

		public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
		{
			if (projectile.ModProjectile is IExplosive explosive)
				projectile.Explode(explosive.BlastRadius);
		}

	}
}