using Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Unfriendly{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class Portal : ModProjectile{
		private int defWidth;
		private int defHeight;

		private bool spawned;

		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Lunar Portal");
		}

		public override void SetDefaults(){
			defWidth = defHeight = projectile.width = projectile.height = 40;
			projectile.hostile = true;
			projectile.friendly = false;
			projectile.tileCollide = false;
			projectile.timeLeft = CraterDemon.PortalTimerMax;
			projectile.penetrate = -1;
			projectile.alpha = 255;
		}

		public override void AI(){
			if(!spawned){
				spawned = true;
				projectile.ai[0] = 255f;
				Main.PlaySound(SoundID.Item78.WithVolume(0.65f), projectile.Center);
			}

			projectile.rotation -= MathHelper.ToRadians(3.6f);

			if(projectile.timeLeft >= CraterDemon.PortalTimerMax - 90)
				projectile.ai[0] -= 2.83333325f;
			else if(projectile.timeLeft <= 90)
				projectile.ai[0] += 2.83333325f;
			else{
				projectile.ai[0] = 0f;
				if(projectile.timeLeft % 14 == 0){
					Projectile.NewProjectile(projectile.Center, (-Vector2.UnitY).RotatedByRandom(20) * Main.rand.NextFloat(6f, 9.25f), ModContent.ProjectileType<Meteor>(), (int)(projectile.damage * 0.4f), projectile.knockBack, projectile.owner, 0f, 0f);

					Main.PlaySound(SoundID.Item20.WithVolume(0.3f), projectile.Center);
				}
			}

			projectile.alpha = (int)projectile.ai[0];

			Vector2 center = projectile.Center;
			projectile.scale = 0.05f + 0.95f * (1f - projectile.alpha / 255f);
			projectile.width = (int)(defWidth * projectile.scale);
			projectile.height = (int)(defHeight * projectile.scale);
			projectile.Center = center;
		}

		public override Color? GetAlpha(Color lightColor)
			=> Color.White * (1f - projectile.alpha / 255f);

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture2D = Main.projectileTexture[projectile.type];
			Color value = GetAlpha(lightColor) ?? Color.White;
			spriteBatch.Draw(texture2D, projectile.Center - Main.screenPosition, null, value * 0.4f, (0f - projectile.rotation) * 0.65f, texture2D.Size() / 2f, projectile.scale * 1.23f, SpriteEffects.FlipHorizontally, 0f);
			spriteBatch.Draw(texture2D, projectile.Center - Main.screenPosition, null, value * 0.84f, projectile.rotation, texture2D.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}
