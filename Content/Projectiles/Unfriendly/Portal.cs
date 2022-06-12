﻿using Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Unfriendly{
	//Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
	public class Portal : ModProjectile {
		private int defWidth;
		private int defHeight;

		private bool spawned;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Lunar Portal");
		}

		public override void SetDefaults() {
			defWidth = defHeight = Projectile.width = Projectile.height = 40;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = CraterDemon.PortalTimerMax;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
		}

		public override void AI() {
			if (!spawned) {
				spawned = true;
				Projectile.ai[0] = 255f;
				Terraria.Audio.SoundEngine.PlaySound(SoundID.Item78, Projectile.Center);
			}

			Projectile.rotation -= MathHelper.ToRadians(3.6f);

			if (Projectile.timeLeft >= CraterDemon.PortalTimerMax - 90)
				Projectile.ai[0] -= 2.83333325f;
			else if (Projectile.timeLeft <= 90)
				Projectile.ai[0] += 2.83333325f;
			else {
				Projectile.ai[0] = 0f;
				if (Projectile.timeLeft % 14 == 0) {
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (-Vector2.UnitY).RotatedByRandom(20) * Main.rand.NextFloat(6f, 9.25f), ModContent.ProjectileType<Meteor>(),
						(int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner, 0f, 0f);

					Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
				}
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
	}
}
