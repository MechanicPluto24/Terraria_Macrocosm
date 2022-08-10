using Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Unfriendly
{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class MeteorPortal : ModProjectile
	{
		private int defWidth;
		private int defHeight;

		private bool spawned;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meteor Portal");
		}

		public override void SetDefaults()
		{
			defWidth = defHeight = Projectile.width = Projectile.height = 40;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = CraterDemon.PortalTimerMax;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
		}

		public override void AI()
		{
			if (!spawned)
			{
				spawned = true;
				Projectile.ai[0] = 255f;
				Terraria.Audio.SoundEngine.PlaySound(SoundID.Item78, Projectile.Center);
			}

			Projectile.rotation -= MathHelper.ToRadians(3.6f);

			if (Projectile.timeLeft >= CraterDemon.PortalTimerMax - 90)
				Projectile.ai[0] -= 2.83333325f;
			else if (Projectile.timeLeft <= 90)
				Projectile.ai[0] += 2.83333325f;
			else
			{
				Projectile.ai[0] = 0f;
				if (Projectile.timeLeft % 14 == 0)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (-Vector2.UnitY).RotatedByRandom(20) * Main.rand.NextFloat(6f, 9.25f), ModContent.ProjectileType<FlamingMeteor>(),
						(int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner, 0f, 0f);

					Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
				}
				SpawnDusts();
			}

			Projectile.alpha = (int)Projectile.ai[0];

			Vector2 center = Projectile.Center;
			Projectile.scale = 0.05f + 0.95f * (1f - Projectile.alpha / 255f);
			Projectile.width = (int)(defWidth * Projectile.scale);
			Projectile.height = (int)(defHeight * Projectile.scale);
			Projectile.Center = center;


			
		}

		public override Color? GetAlpha(Color lightColor)
			=> Color.White * (1f - Projectile.alpha / 255f);

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D = TextureAssets.Projectile[Projectile.type].Value;
			Color value = GetAlpha(lightColor) ?? Color.White;
			Main.EntitySpriteDraw(texture2D, Projectile.Center - Main.screenPosition, null, value * 0.4f, (0f - Projectile.rotation) * 0.65f, texture2D.Size() / 2f,
				Projectile.scale * 1.23f, SpriteEffects.FlipHorizontally, 0);
			Main.EntitySpriteDraw(texture2D, Projectile.Center - Main.screenPosition, null, value * 0.84f, Projectile.rotation, texture2D.Size() / 2f,
				Projectile.scale, SpriteEffects.None, 0);
			return false;
		}


		/// <summary>
		/// Copied from Lunar Portal Staff
		/// </summary>
		private void SpawnDusts()
		{
			if (Main.rand.Next(2) == 0)
			{
				Vector2 vector95 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
				Dust dust47 = Main.dust[Dust.NewDust(Projectile.Center - vector95 * 30f, 0, 0, 229)];
				dust47.noGravity = true;
				dust47.position = Projectile.Center - vector95 * Main.rand.Next(10, 21);
				dust47.velocity = vector95.RotatedBy(1.5707963705062866) * 6f;
				dust47.scale = 0.5f + Main.rand.NextFloat();
				dust47.fadeIn = 0.5f;
				dust47.customData = Projectile.Center;
				dust47.color = new Color(0, 255, 0); // doesn't work 
			}

			if (Main.rand.Next(2) == 0)
			{
				Vector2 vector96 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
				Dust dust48 = Main.dust[Dust.NewDust(Projectile.Center - vector96 * 30f, 0, 0, 240)];
				dust48.noGravity = true;
				dust48.position = Projectile.Center - vector96 * 30f;
				dust48.velocity = vector96.RotatedBy(-1.5707963705062866) * 3f;
				dust48.scale = 0.5f + Main.rand.NextFloat();
				dust48.fadeIn = 0.5f;
				dust48.customData = Projectile.Center;
				dust48.color = new Color(0, 255, 0); // doesn't work 
			}

			if (Projectile.ai[0] < 0f)
			{
				Vector2 center13 = Projectile.Center;
				int num932 = Dust.NewDust(center13 - Vector2.One * 8f, 16, 16, 229, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f);
				Dust dust2 = Main.dust[num932];
				dust2.velocity *= 2f;
				Main.dust[num932].noGravity = true;
				Main.dust[num932].scale = Utils.SelectRandom<float>(Main.rand, 0.8f, 1.65f);
				Main.dust[num932].customData = this;
				Main.dust[num932].color = new Color(0, 255, 0); // doesn't work 
			}
		}
	}
}
