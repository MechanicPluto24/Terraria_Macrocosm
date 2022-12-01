using Macrocosm.Common.Drawing;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
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

			Projectile.rotation -= MathHelper.ToRadians(5.4f);

			if (Projectile.timeLeft >= CraterDemon.PortalTimerMax - 90)
				Projectile.ai[0] -= 2.83333325f;
			else if (Projectile.timeLeft <= 90)
				Projectile.ai[0] += 2.83333325f;
			else
			{
				Projectile.ai[0] = 0f;
				if (Projectile.timeLeft % 14 == 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int meteorID = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (-Vector2.UnitY).RotatedByRandom(20) * Main.rand.NextFloat(6f, 9.25f), ModContent.ProjectileType<FlamingMeteor>(),
							(int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner, 0f, 0f);
						Main.projectile[meteorID].netUpdate = true;
					}

					Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
				}
				SpawnDusts();
			}

			Projectile.alpha = (int)MathHelper.Clamp((int)Projectile.ai[0], 0f, 255f);

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

			SpriteBatchState state = Main.spriteBatch.SaveState();

			Main.spriteBatch.EndIfBeginCalled();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, state);

			Main.EntitySpriteDraw(texture2D, Projectile.Center - Main.screenPosition, null, (value * 0.2f).NewAlpha(1f - Projectile.alpha/255f), (0f - Projectile.rotation) * 0.65f, texture2D.Size() / 2f,
				Projectile.scale * 1.23f, SpriteEffects.FlipHorizontally, 0);

			Main.EntitySpriteDraw(texture2D, Projectile.Center - Main.screenPosition, null, value * 0.84f, Projectile.rotation, texture2D.Size() / 2f,
				Projectile.scale, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, state);

			Main.EntitySpriteDraw(texture2D, Projectile.Center - Main.screenPosition, null, value * 1f, (0f - Projectile.rotation) * 0.65f, texture2D.Size() / 2f,
			Projectile.scale * 0.8f, SpriteEffects.None, 0);

			Main.spriteBatch.Restore(state);

			return false;
		}

		/// <summary> Adapted from Lunar Portal Staff </summary>
		private void SpawnDusts()
		{
			if (Main.rand.NextBool(1))
			{
				int type = Main.rand.NextBool() ? ModContent.DustType<PortalLightOrangeDust>() : ModContent.DustType<PortalLightGreenDust>();
				Vector2 rotVector1 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
				Dust lightDust = Main.dust[Dust.NewDust(Projectile.Center - rotVector1 * 30f, 0, 0, type)];
				lightDust.noGravity = true;
				lightDust.position = Projectile.Center - rotVector1 * Main.rand.Next(10, 21);
				lightDust.velocity = rotVector1.RotatedBy(MathHelper.PiOver2) * 6f;
				lightDust.scale = 1.2f + Main.rand.NextFloat();
				lightDust.fadeIn = 0.5f;
				lightDust.customData = Projectile.Center;

			}

			if (Main.rand.NextBool(2))
			{
				int type = Main.rand.NextBool() ? ModContent.DustType<PortalDarkOrangeDust>() : ModContent.DustType<PortalDarkGreenDust>();
				Vector2 rotVector2 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
				Dust darkDust = Main.dust[Dust.NewDust(Projectile.Center - rotVector2 * 30f, 0, 0, type)];
				darkDust.noGravity = true;
				darkDust.position = Projectile.Center - rotVector2 * 30f;
				darkDust.velocity = rotVector2.RotatedBy(-MathHelper.PiOver2) * 3f;
				darkDust.scale = 1.2f + Main.rand.NextFloat();
				darkDust.fadeIn = 0.5f;
				darkDust.customData = Main.projectile[Projectile.whoAmI];
			}
		}
	}
}
