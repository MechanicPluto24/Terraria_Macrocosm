using Macrocosm.Common.Global.GlobalProjectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class ArchersLineProjectile : ModProjectile, IBullet
	{
		public override string Texture => "Terraria/Images/Projectile_14";
		private readonly bool[] hitList = new bool[Main.maxNPCs]; //Used to keep track of every NPC hit

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Type] = 10;
			ProjectileID.Sets.TrailingMode[Type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(14);
			AIType = ProjectileID.Bullet;
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.timeLeft = 600;
			Projectile.penetrate = 10;
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.DrawSimpleTrail(Vector2.Zero, 2f, 0.5f, new Color(254, 121, 2) * lightColor.GetLuminance(), new Color(184, 58, 24, 0) * lightColor.GetLuminance());
			return true;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.damage -= 10;

			hitList[target.whoAmI] = true; //Make sure the projectile won't aim directly for this NPC
			int newTarget = GetTarget(600, Projectile.Center); //Keeps track of the current target, set to -1 to ensure no NPC by default

			if (newTarget != -1)
			{
				if (Projectile.owner == Main.myPlayer)
				{
					Vector2 shootVel = Main.npc[newTarget].Center - Projectile.Center;
					shootVel.Normalize();
					shootVel *= 20f;
					Projectile.velocity = shootVel;
					Projectile.rotation = Main.npc[newTarget].Center.ToRotation();
				}

				SoundEngine.PlaySound(SFX.Ricochet with { Volume = 0.3f });
			}

			Projectile.velocity *= 0.9f;
			Projectile.netUpdate = true;
		}

		public int GetTarget(float maxRange, Vector2 shootingSpot) //Function to find a NPC to target
		{
			int first = -1; //Used to keep track of the closest NPC, rather than the first one based on NPC whoAmI
			for (int j = 0; j < Main.maxNPCs; j++)
			{
				NPC npc = Main.npc[j];
				if (npc.CanBeChasedBy(this, false) && !hitList[j])
				{
					float distance = Vector2.Distance(shootingSpot, npc.Center);
					if (distance <= maxRange)
					{
						if ((first == -1 || distance < Vector2.Distance(shootingSpot, Main.npc[first].Center)) && Collision.CanHitLine(shootingSpot, 0, 0, npc.Center, 0, 0))
						{
							first = j;
						}
					}
				}
			}
			return first;
		}
	}
}
