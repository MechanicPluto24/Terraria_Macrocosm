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
	public class ArtemiteGreatswordSwing : ModProjectile
	{
		public override void SetStaticDefaults()
		{
 			DisplayName.SetDefault("Artemite Greatsword");
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

		private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Microsoft.Xna.Framework.Color drawColor, Microsoft.Xna.Framework.Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
		{
			Texture2D value = TextureAssets.Extra[89].Value;
			Microsoft.Xna.Framework.Color color = shineColor * opacity * 0.5f;
			color.A = 0;
			Vector2 origin = value.Size() / 2f;
			Microsoft.Xna.Framework.Color color2 = drawColor * 0.5f;
			float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num;
			color *= num;
			color2 *= num;
			//Main.EntitySpriteDraw(value, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir, 1);
			//Main.EntitySpriteDraw(value, drawpos, null, color, 0f + rotation, origin, vector2, dir, 1);
			Main.EntitySpriteDraw(value, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir, 1);
			Main.EntitySpriteDraw(value, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir, 1);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color color = Color.White * 0.5f;
			Color color2 = color * 0.8f;
			Color color3 = color * 0.7f;

			SpriteBatchState state = Main.spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, state);

			Vector2 vector = Projectile.Center - Main.screenPosition;
			Asset<Texture2D> val = TextureAssets.Projectile[Projectile.type];
			Rectangle rectangle = val.Frame(1, 4);
			Vector2 origin = rectangle.Size() / 2f;
			float num = Projectile.scale * 1.1f;
			SpriteEffects effects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
			float num2 = Projectile.localAI[0] / Projectile.ai[1];
			float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
			float num4 = 0.975f;
			float num5 = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
			num5 = 0.5f + num5 * 0.5f;
			num5 = Utils.Remap(num5, 0.2f, 1f, 0f, 1f);
			Color color4 = Color.White * num3 * 0.5f;
			Color color5 = color4 * num5 * 0.5f;
			Main.spriteBatch.Draw(val.Value, vector, rectangle, color * num5 * num3, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - num2), origin, num * 0.95f, effects, 0f);
			color4.A = (byte)((float)(int)color4.A * (1f - num5));
			color5.G = (byte)((float)(int)color5.G * num5);
			color5.B = (byte)((float)(int)color5.R * (0.25f + num5 * 0.75f));
			Main.spriteBatch.Draw(val.Value, vector, rectangle, color5 * 0.15f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, num, effects, 0f);
			Main.spriteBatch.Draw(val.Value, vector, rectangle, color3 * num5 * num3 * 0.3f, Projectile.rotation, origin, num, effects, 0f);
		    Main.spriteBatch.Draw(val.Value, vector, rectangle, color2 * num5 * num3 * 0.5f, Projectile.rotation, origin, num * num4, effects, 0f);
			//Main.spriteBatch.Draw(val.Value, vector, val.Frame(1, 4, 0, 3), Color.White * 0.6f * num3, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, num * 0.97f, effects, 0f);
			//Main.spriteBatch.Draw(val.Value, vector, val.Frame(1, 4, 0, 3), Color.White * 0.5f * num3, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, num * 0.8f, effects, 0f);
			//Main.spriteBatch.Draw(val.Value, vector, val.Frame(1, 4, 0, 3), Color.White * 0.4f * num3, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, num * 0.6f, effects, 0f);


 			for (float num6 = 0f; num6 < 16f; num6 += 0.75f)
			{
				float num7 = Projectile.rotation + Projectile.ai[0] * (num6 - 2f) * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 2.4f) * Projectile.ai[0];
				Vector2 drawpos = vector + num7.ToRotationVector2() * ((float)val.Width() * 0.5f - 8.4f) * num;
 				DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos, new Microsoft.Xna.Framework.Color(255, 255, 255, 0) * num3 * 0.8f * (float)(num6 / 8f), color3, num2, 0f, 0.5f, 0.5f, 1f, num7, new Vector2(0.1f, Utils.Remap(num2, 0f, 1f, 2f, 0f)) * num, Vector2.One * num * 1.2f);
			
				//if(num6 < 10f)
				//{
				//	num7 = Projectile.rotation + Projectile.ai[0] * (num6 - 2f) * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 14f) * Projectile.ai[0];
				//	drawpos = vector + num7.ToRotationVector2() * ((float)val.Width() * 0.5f - 22f) * num;
				//	DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos, new Microsoft.Xna.Framework.Color(255, 255, 255, 0) * num3 * 0.6f * (float)(num6 / 12f), color3, num2, 0f, 0.5f, 0.5f, 1f, num7, new Vector2(0f, Utils.Remap(num2, 0f, 1f, 2f, 0f)) * num, Vector2.One * num * 1f);
				//	
				//	num7 = Projectile.rotation + Projectile.ai[0] * (num6 - 2f) * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 36f) * Projectile.ai[0];
				//	drawpos = vector + num7.ToRotationVector2() * ((float)val.Width() * 0.5f - 34f) * num;
				//	DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos, new Microsoft.Xna.Framework.Color(255, 255, 255, 0) * num3 * 0.4f * (float)(num6 / 16f), color3, num2, 0f, 0.5f, 0.5f, 1f, num7, new Vector2(0f, Utils.Remap(num2, 0f, 1f, 2f, 0f)) * num, Vector2.One * num * 0.5f);
				//}
			
				
			}
			//Vector2 drawpos2 = vector + (Projectile.rotation + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0]).ToRotationVector2() * ((float)val.Width() * 0.5f - 4f) * num;
			//DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos2, new Microsoft.Xna.Framework.Color(255, 255, 255, 0) * num3 * 0.5f, color3, num2, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(1f, Utils.Remap(num2, 0f, 1f, 0.5f, 1f)) * num, Vector2.One * num * 1.5f);


			Main.spriteBatch.End();
			Main.spriteBatch.Restore(state);
			
			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			base.PostDraw(lightColor);
		}

		public override void AI()
		{
			Projectile.localAI[0] += 1f;
			Player player = Main.player[Projectile.owner];
			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float direction = Projectile.ai[0];
			float rotation = Projectile.velocity.ToRotation();
			Projectile.rotation = (float)Math.PI * direction * progress + rotation + direction * (float)Math.PI + player.fullRotation;
			float scaleFactor = 0.2f;
			float baseScale = 1.2f;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity;
			Projectile.scale = baseScale + progress * scaleFactor;
			Projectile.scale *= Main.player[Projectile.owner].GetAdjustedItemScale(Main.item[Main.player[Projectile.owner].selectedItem]);

			if (Projectile.localAI[0] >= Projectile.ai[1])
 				Projectile.Kill();
 		}
	}
}