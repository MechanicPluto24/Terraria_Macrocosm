using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Macrocosm.Common.Drawing
{
	public class CelestialBody 
	{
		public bool HasAtmo => atmoTexture is not null;
		public bool HasShadow => bodyShadowTexture is not null;
		public bool HasAtmoShadow => atmoShadowTexture is not null;
		public bool IsParallaxing => parallaxSpeedX > 0f || parallaxSpeedY > 0f;
		public bool HasScreenCenterOffset => averageOffset != Vector2.Zero;
		public bool IsRotating { get; set; }
		public bool IsRotatingInDaytime { get; set; }

		private Texture2D bodyTexture;
		private Texture2D atmoTexture;

		private Vector2 averageOffset = default;
		private float parallaxSpeedX = 0f;
		private float parallaxSpeedY = 0f;
		private float scale;
		private float rotation;
		private Color color;

		private Vector2 screenPosition;

		private Texture2D bodyShadowTexture = null;
		private Texture2D atmoShadowTexture = null;
		private CelestialBody lightSource = null;
		private float shadowRotation = 0f;
		private Color shadowColor = default;

		public CelestialBody(Texture2D bodyTexture, Texture2D atmoTexture = null, float scale = 1f, float rotation = 0f)
		{
			this.bodyTexture = bodyTexture;
			this.scale = scale;
			this.atmoTexture = atmoTexture;
			this.rotation = rotation;
		}

		public CelestialBody(Texture2D bodyTexture, Texture2D atmoTexture = null, Vector2 screenPosition = default, float scale = 1f, float rotation = 0f)
		{
			this.bodyTexture = bodyTexture;
			this.atmoTexture = atmoTexture;
			this.scale = scale;
			this.rotation = rotation;

			OverrideScreenPosition(screenPosition);
		}

		public void OverrideScreenPosition(Vector2 position)
		{
			averageOffset = default;
			screenPosition = position;
		}

		public void SetBackgroundSurfaceParams(Vector2 averageOffset, float parallaxSpeedX, float parallaxSpeedY)
		{
			SetAverageOffset(averageOffset);
			SetParallax(parallaxSpeedX, parallaxSpeedY);
		}

		public void SetupShadow(CelestialBody lightSource, Texture2D bodyShadowTexture, Texture2D atmoShadowTexture)
		{
			this.lightSource = lightSource;
			this.bodyShadowTexture = bodyShadowTexture;
			this.atmoShadowTexture = atmoShadowTexture;
		}

		public void SetupRotation(bool dayTime)
		{
			IsRotating = true;
			IsRotatingInDaytime = dayTime;
		}

		public void SetAverageOffset(Vector2 offset) => averageOffset = offset;
		public void SetParallax(float speedX, float speedY)
		{
			parallaxSpeedX = speedX;
			parallaxSpeedY = speedY;
		}

		/// <summary>
		/// Draws a celestial body with an atmosphere at the screen center, with possible offsets and parallax speeds
		/// </summary>
		/// <param name="bodyTexture"> The celestial body to draw </param>
		/// <param name="atmoTexture"> The celestial body's atmosphere </param>
		/// <param name="scale"> The scale of the texture </param>
		/// <param name="averageOffsetX"> The offset from screen center when the player is in the world's horizontal center </param>
		/// <param name="averageOffsetY"> The offset from screen center when the player is midway between world's surface and upper bounds </param>
		/// <param name="parallax_X"> The horizontal parallax speed relative to the player </param>
		/// <param name="parallax_Y"> The vertical parallax speed relative to the player </param>
		public void DrawSelf(SpriteBatch spriteBatch)
		{
			if (IsParallaxing || HasScreenCenterOffset)
			{
				// surface layer dimensions in game coordinates 
				float worldWidth = Main.maxTilesX * 16f;
				float surfaceLayerHeight = (float)Main.worldSurface * 16f;

				// positions relative to the center origin of the surface layer 
				float playerPositionToCenterX = Main.LocalPlayer.position.X - (worldWidth / 2);
				float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - (surfaceLayerHeight / 2);

				screenPosition = new Vector2(
					(Main.screenWidth / 2) - playerPositionToCenterX * parallaxSpeedX + averageOffset.X,
					(Main.screenHeight / 2) - playerPositionToSurfaceCenterY * parallaxSpeedY + averageOffset.Y
				);
			}
			else if (IsRotating)
			{
				double duration = Main.dayTime ? Main.dayLength : Main.nightLength;
				double bgTop = -Main.screenPosition.Y / (Main.worldSurface * 16.0 - 600.0) * 200.0;

				int timeX = (int)(Main.time / duration * (Main.screenWidth + bodyTexture.Width * 2)) - bodyTexture.Width;
				double timeY = Main.time < duration / 2 ? //Gets the Y axis for the angle depending on the time
					Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) : //AM
					Math.Pow(1.0 - Main.time / duration * 2.0, 2.0); //PM

				rotation = (float)(Main.time / duration) * 2f - 7.3f;
				scale *= (float)(1.2 - timeY * 0.4);

				float clouldAlphaMult = Math.Max(0f, 1f - Main.cloudAlpha * 1.5f);
			    color = new Color((byte)(255f * clouldAlphaMult), (byte)(Color.White.G * clouldAlphaMult), (byte)(Color.White.B * clouldAlphaMult), (byte)(255f * clouldAlphaMult));
				int angle = (int)(bgTop + timeY * 250.0 + 180.0);

				OverrideScreenPosition(new Vector2(timeX, angle + Main.sunModY));
			}

			spriteBatch.End();

			#region Atmosphere
			if (atmoTexture is not null)
			{
				// draw atmosphere in Additive BlendState (for proper transparency) and in the EffectMatrix (no scaling with screen size) 
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(atmoTexture, screenPosition, null, color, rotation, atmoTexture.Size() / 2, scale, default, 0f);
				spriteBatch.End();
			}
			#endregion

			#region Body
			// draw body in the EffectMatrix 
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(bodyTexture, screenPosition, null, color, rotation, bodyTexture.Size() / 2, scale, default, 0f);
			spriteBatch.End();
			#endregion

			#region Atmo Shadow

			if(lightSource is not null)
			{
				shadowRotation = (screenPosition - lightSource.screenPosition).ToRotation();

				if (!Main.dayTime)
					shadowRotation -= MathHelper.Pi;

				shadowColor = Color.White;
				shadowColor.A = (byte)(255f * ScaleBrightnessNoonToMidnight());

				if (atmoShadowTexture is not null)
				{
					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
					spriteBatch.Draw(atmoShadowTexture, screenPosition, null, shadowColor, shadowRotation, atmoTexture.Size() / 2, scale, default, 0f);
					spriteBatch.End();
				}
				#endregion

				#region
				if (bodyShadowTexture is not null)
				{
					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
					spriteBatch.Draw(bodyShadowTexture, screenPosition, null, shadowColor, shadowRotation, atmoTexture.Size() / 2, scale, default, 0f);
					spriteBatch.End();
				}
			}

			#endregion

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public static float ScaleBrigthness(bool dayTime, double timeHigh, double timeLow, float minBrightness, float maxBrightness)
		{
			float brightness;
			float fadeFactor = maxBrightness - minBrightness;

			if (dayTime == Main.dayTime)
				if (Main.time <= timeLow)
					brightness = (minBrightness + ((1f - (float)(Main.time / timeLow)) * fadeFactor));
				else if (Main.time >= timeHigh)
					brightness = (minBrightness + (float)((Main.time - timeHigh) / timeLow) * fadeFactor);
				else
					brightness = minBrightness;
			else
				brightness = maxBrightness;

			return brightness;
		}

		public static float ScaleBrightnessNoonToMidnight()
		{
			float brightness;
			double totalTime = Main.dayTime ? Main.time : Main.dayLength + Main.time;

			if (totalTime <= 27000)
				brightness = (byte)(0.4f + 0.6f * ((totalTime) / 27000));
			else if (totalTime >= 70200)
				brightness = (byte)(0.4f * ((totalTime - 70200) / 16200));
			else
				brightness = (byte)(1f - ((totalTime - 27000) / 43200));

			return brightness;
		}

	}
}