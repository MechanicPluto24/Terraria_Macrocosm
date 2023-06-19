using Macrocosm.Content.Projectiles.Global;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Base;
using System;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	/*
    public class Tycho50Projectile : HeldGunProjectile
	{

		public ref float AITimer => ref Projectile.ai[0]; 

		public override void SetProjectileStaticDefaults()
		{

		}


		public override void SetProjectileDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 32;
			Projectile.timeLeft = 10;
 		}


		public override void ProjectileAI()
		{
			Utility.Chat(Projectile.rotation.ToString());

			AITimer++;

			float attackSpeed = OwnerPlayer.GetAttackSpeed(DamageClass.Ranged);
			int shootTime = (int)(15 * 1 / attackSpeed);
			int resetTime = (int)(30 * 1 / attackSpeed);

			if (AITimer == shootTime)
			{
				int damage = OwnerPlayer.GetWeaponDamage(OwnerPlayer.inventory[OwnerPlayer.selectedItem]);
				int projToShoot = ModContent.ProjectileType<Tycho50Bullet>();
				float knockback = OwnerPlayer.inventory[OwnerPlayer.selectedItem].knockBack;
				Vector2 rotPoint = Utility.RotatingPoint(Projectile.Center, new Vector2(40, 8 * Projectile.spriteDirection), Projectile.rotation);

				Projectile bullet = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), rotPoint, Vector2.Normalize(Projectile.velocity) * 10f, projToShoot, damage, knockback, Projectile.owner, default, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));
				Particle.CreateParticle<DesertEagleFlash>(bullet.position + bullet.velocity * 0.24f, bullet.velocity * 0.5f, bullet.velocity.ToRotation(), 1f, false);

				//Projectile.velocity = new Vector2(10f);
			}

			if (AITimer == resetTime) {

				if(!StillInUse)
					Projectile.Kill();
				else
					AITimer = 0;
			}				
		}
	}*/
}
