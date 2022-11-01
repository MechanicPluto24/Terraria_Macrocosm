using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
	public class MoonSwordSwing : ModProjectile
	{
		public override void SetStaticDefaults()
		{
 			DisplayName.SetDefault("Moonlight Greatsword");
		}

		public override string Texture => "Macrocosm/Assets/Textures/Swing";

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.ownerHitCheck = true;
			Projectile.ownerHitCheckDistance = 300f;
			//Projectile.usesOwnerMeleeHitCD = true;
			//Projectile.stopsDealingDamageAfterPenetrateHits = true;
		}

		public override void PostDraw(Color lightColor)
		{
			base.PostDraw(lightColor);
		}

		public override void AI()
		{
			float scaleFactor = 0.2f;
			float baseScale = 1.45f;
			float direction = Projectile.ai[0];
			float progress = Projectile.localAI[0] / Projectile.ai[1];
			Player player = Main.player[Projectile.owner];

			Projectile.localAI[0] += 1f;

			Projectile.rotation = (float)Math.PI * direction * progress + Projectile.velocity.ToRotation() + direction * (float)Math.PI + player.fullRotation;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity;
			Projectile.scale = baseScale + progress * scaleFactor;
			//Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.item[Main.player[Projectile.owner].selectedItem]);

			if (Projectile.localAI[0] >= Projectile.ai[1])
 				Projectile.Kill();
 		}

		public override bool PreDraw(ref Color lightColor)
		{
			Asset<Texture2D> val = TextureAssets.Projectile[Type];

			Rectangle frame = val.Frame(1, 4);
			Vector2 origin = frame.Size() / 2f;

			Vector2 position = Projectile.Center - Main.screenPosition;
			float scale = Projectile.scale * 1.15f;
			SpriteEffects effects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);

			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

			Color color = new Color(81, 180, 114);
			Color color2 = new Color(157, 255, 201);
			Color color3 = new Color(33, 109, 85);

			float brightness = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
			brightness = 0.5f + brightness * 0.5f;
			brightness = Utils.Remap(brightness, 0.2f, 1f, 0f, 1f);

			Color color4 = Color.White * progressScale * 0.5f;
			Color color5 = color4 * brightness * 0.5f;

			SpriteBatchState state = Main.spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, state);

			Vector2 starPos = position + (Projectile.rotation + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0]).ToRotationVector2() * ((float)val.Width() * 0.5f - 4f) * scale;
			DrawParticles(Projectile.Opacity, SpriteEffects.None, starPos, color * progressScale * 0.5f, color3, progress, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(1f, Utils.Remap(progress, 0f, 1f, 0.5f, 1f)) * scale, Vector2.One * scale * 1.5f);

			Main.spriteBatch.Draw(val.Value, position, frame, color * brightness * progressScale, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, scale * 0.95f, effects, 0f);

			color4.A = (byte)((float)(int)color4.A * (1f - brightness));
			color5.G = (byte)((float)(int)color5.G * brightness);
			color5.B = (byte)((float)(int)color5.R * (0.25f + brightness * 0.75f));

			Main.spriteBatch.Draw(val.Value, position, frame, color5 * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, scale, effects, 0f);
			Main.spriteBatch.Draw(val.Value, position, frame, color3 * brightness * progressScale * 0.3f, Projectile.rotation, origin, scale, effects, 0f);
		    Main.spriteBatch.Draw(val.Value, position, frame, color2 * brightness * progressScale * 0.5f, Projectile.rotation, origin, scale * 0.975f, effects, 0f);

 			for (float f = 0f; f < 16f; f += 0.5f)
			{
				float angle = Projectile.rotation + Projectile.ai[0] * (f - 2f) * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 2.8f) * Projectile.ai[0];
				Vector2 drawpos = position + angle.ToRotationVector2() * ((float)val.Width() * 0.5f - 8f) * scale;
 				DrawParticles(Projectile.Opacity, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0) * progressScale * 0.8f * (float)(f / 8f), color3, progress, 0f, 0.5f, 0.5f, 1f, angle, new Vector2(0.1f, Utils.Remap(progress, 0f, 1f, 2f, 0f)) * scale, Vector2.One * scale * 1.2f);

				if(f < 10f)
				{
					angle = Projectile.rotation + Projectile.ai[0] * (f - 2f) * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 14f) * Projectile.ai[0];
					drawpos = position + angle.ToRotationVector2() * ((float)val.Width() * 0.5f - 22f) * scale;
					DrawParticles(Projectile.Opacity, SpriteEffects.None, drawpos, color * progressScale * 0.6f * (float)(f / 12f), color3, progress, 0f, 0.5f, 0.5f, 1f, angle, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 2f, 0f)) * scale, Vector2.One * scale * 1f);

					angle = Projectile.rotation + Projectile.ai[0] * (f - 2f) * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(progress, 0f, 1f, 0f, (float)Math.PI / 36f) * Projectile.ai[0];
					drawpos = position + angle.ToRotationVector2() * ((float)val.Width() * 0.5f - 34f) * scale;
					DrawParticles(Projectile.Opacity, SpriteEffects.None, drawpos, color * progressScale * 0.4f * (float)(f / 16f), color3, progress, 0f, 0.5f, 0.5f, 1f, angle, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 2f, 0f)) * scale, Vector2.One * scale * 0.5f);
				}
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Restore(state);
			
			return false;
		}

		private static void DrawParticles(float opacity, SpriteEffects dir, Vector2 drawpos, Microsoft.Xna.Framework.Color drawColor, Microsoft.Xna.Framework.Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
		{
			Texture2D texture = TextureAssets.Extra[89].Value;
			Vector2 origin = texture.Size() / 2f;

			float fade = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);

			Color color = (shineColor * opacity * 0.5f).NewAlpha(0f) * fade;
			Color color2 = drawColor * 0.5f * fade;

			Vector2 scaleX = new Vector2(fatness.X * 0.5f, scale.X) * fade;
			Vector2 scaleY = new Vector2(fatness.Y * 0.5f, scale.Y) * fade;

			Main.EntitySpriteDraw(texture, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, scaleX, dir, 1);
			Main.EntitySpriteDraw(texture, drawpos, null, color, 0f + rotation, origin, scaleY, dir, 1);
			Main.EntitySpriteDraw(texture, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, scaleX * 0.6f, dir, 1);
			Main.EntitySpriteDraw(texture, drawpos, null, color2, 0f + rotation, origin, scaleY * 0.6f, dir, 1);
		}
	}
}