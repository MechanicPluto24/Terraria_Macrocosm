using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Macrocosm.Common.Drawing
{
	public class CelestialObject
	{
		public enum SkyRotationMode
		{
			None,
			Day,
			Night,
			Any
		}
		public bool HasAtmo => atmoTexture is not null;
		public bool HasShadow { get; set; } = false;

		private Texture2D bodyTexture;
		private Texture2D atmoTexture;

		private Vector2 averageOffset = default;
		private float parallaxSpeedX = 0f;
		private float parallaxSpeedY = 0f;
		private float scale;
		private float rotation;
		private Color color = Color.White;

		private Vector2 screenPosition;

		private SkyRotationMode rotationMode = SkyRotationMode.None;

		private Texture2D bodyShadowTexture = null;
		private Texture2D atmoShadowTexture = null;
		private CelestialObject lightSource = null;
		private float shadowRotation = 0f;
		private Color shadowColor = Color.White;

		public CelestialObject(Texture2D bodyTexture, Texture2D atmoTexture = null, float scale = 1f, float rotation = 0f)
		{
			this.bodyTexture = bodyTexture;
			this.scale = scale;
			this.atmoTexture = atmoTexture;
			this.rotation = rotation;
		}

		public void SetScreenPosition(Vector2 position) => screenPosition = position;
		public void SetScreenPosition(float x, float y) => SetScreenPosition(new Vector2(x, y));


		/// <summary>
		/// Configure parallax settings for the object
		/// </summary>
		/// <param name="parallaxX"> Horizontal parallaxing speed  </param>
		/// <param name="parallaxY"> Vertical parallaxing speed </param>
		/// <param name="averageOffset"> The offset from the screen center when the player is in the middle of the world </param>
		public void SetParallax(float parallaxX = 0f, float parallaxY = 0f, Vector2 averageOffset = default)
		{
			this.parallaxSpeedX = parallaxX;
			this.parallaxSpeedY = parallaxY;
			this.averageOffset = averageOffset;
		}

		private void Parallax()
		{
			// surface layer dimensions in game coordinates 
			float worldWidth = Main.maxTilesX * 16f;
			float surfaceLayerHeight = (float)Main.worldSurface * 16f;

			// positions relative to the center origin of the surface layer 
			float playerPositionToCenterX = Main.LocalPlayer.position.X - (worldWidth / 2);
			float playerPositionToSurfaceCenterY = Main.LocalPlayer.position.Y - (surfaceLayerHeight / 2);

			SetScreenPosition(new Vector2(
				(Main.screenWidth / 2) - playerPositionToCenterX * parallaxSpeedX + averageOffset.X,
				(Main.screenHeight / 2) - playerPositionToSurfaceCenterY * parallaxSpeedY + averageOffset.Y
			));
		}

		/// <summary>
		/// Configures rotation of the object on the sky 
		/// </summary>
		/// <param name="mode">
		/// None  - No rotation
		/// Day   - Only visible during the day (rotation logic will still run)
		/// Night - Only visible during the night (rotation logic will still run)
		/// Any   - Cycle during both day and night  
		/// </param>
		public void SetSkyRotationMode(SkyRotationMode mode) => rotationMode = mode;

		/// <summary>
		/// Configures shading for the object 
		/// </summary>
		/// <param name="lightSource"> The object the shade will rotate away from (leave null for stationary) </param>
		/// <param name="bodyShadowTexture"> The shadow texture that draws over the object's body (null for none) </param>
		/// <param name="atmoShadowTexture"> The shadow texture that draws over the object's atmosphere (null for none) </param>
		public void SetupShadow(CelestialObject lightSource = null, Texture2D bodyShadowTexture = null, Texture2D atmoShadowTexture = null)
		{
			HasShadow = true;
			this.lightSource = lightSource;
			this.bodyShadowTexture = bodyShadowTexture;
			this.atmoShadowTexture = atmoShadowTexture;
		}

		public void DrawSelf(SpriteBatch spriteBatch)
		{
			// these are mutually exclusive rn :(
			if (parallaxSpeedX > 0f || parallaxSpeedY > 0f || averageOffset != default)
				Parallax(); // stationary parallaxing mode 
			else if (rotationMode != SkyRotationMode.None)
				Rotate(); // rotate even if not drawing, it affects the shadow rotation  

			if (!ShouldDraw())
				return;

			spriteBatch.End();

			#region Atmosphere
			if (atmoTexture is not null)
			{
				// draw atmosphere in Additive BlendState (for proper transparency) and in the EffectMatrix (no scaling with screen size) 
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw(atmoTexture, screenPosition, null, Color.White, rotation, atmoTexture.Size() / 2, scale, default, 0f);
				spriteBatch.End();
			}
			#endregion

			#region Body
			// draw body in the EffectMatrix 
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
			spriteBatch.Draw(bodyTexture, screenPosition, null, Color.White, rotation, bodyTexture.Size() / 2, scale, default, 0f);
			spriteBatch.End();
			#endregion

			#region Shadow
			if (HasShadow)
			{
				if (lightSource is not null)
				{
					shadowRotation = (screenPosition - lightSource.screenPosition).ToRotation();

					if (!Main.dayTime)
						shadowRotation -= MathHelper.Pi;
				}

				shadowColor = Color.White;
				shadowColor.A = (byte)(255f * ScaleBrightnessNoonToMidnight(0f, 1f));

				if (atmoShadowTexture is not null && HasAtmo)
				{
					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
					spriteBatch.Draw(atmoShadowTexture, screenPosition, null, shadowColor, shadowRotation, atmoTexture.Size() / 2, scale, default, 0f);
					spriteBatch.End();
				}

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

		private void Rotate()
		{
			double duration = Main.dayTime ? Main.dayLength : Main.nightLength;
			double bgTop = -Main.screenPosition.Y / (Main.worldSurface * 16.0 - 600.0) * 200.0;

			int timeX = (int)(Main.time / duration * (Main.screenWidth + bodyTexture.Width * 2)) - bodyTexture.Width;
			double timeY = Main.time < duration / 2 ? //Gets the Y axis for the angle depending on the time
				Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) : //AM
				Math.Pow(1.0 - Main.time / duration * 2.0, 2.0); //PM

			rotation = (float)(Main.time / duration) * 2f - 7.3f;
			scale = (float)(1.2 - timeY * 0.4);

			float clouldAlphaMult = Math.Max(0f, 1f - Main.cloudAlpha * 1.5f);
			color = new Color((byte)(255f * clouldAlphaMult), (byte)(Color.White.G * clouldAlphaMult), (byte)(Color.White.B * clouldAlphaMult), (byte)(255f * clouldAlphaMult));
			int angle = (int)(bgTop + timeY * 250.0 + 180.0);

			SetScreenPosition(new Vector2(timeX, angle + Main.sunModY)); // TODO: add configurable vertical parallax 
		}

		private bool ShouldDraw()
		{
			bool shouldDraw;
			if (rotationMode == SkyRotationMode.Day)
				shouldDraw = Main.dayTime;
			else if (rotationMode == SkyRotationMode.Night)
				shouldDraw = !Main.dayTime;
			else
				shouldDraw = true;

			return shouldDraw;
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

		// uhh don't look at this - Feldy
		/// <summary>
		/// Used for linear brightness scaling along an entire day/night cycle  
		/// </summary>
		public static float ScaleBrightnessNoonToMidnight(float minBrightness, float maxBrightness)
		{
			float brightness;
			double totalTime = Main.dayTime ? Main.time : Main.dayLength + Main.time;

			float diff = maxBrightness - minBrightness;

			if (totalTime <= 27000)
				brightness = minBrightness + maxBrightness * (diff * 0.4f + (diff * 0.6f * ((float)(totalTime) / 27000)));
			else if (totalTime >= 70200)
				brightness = (diff * 0.4f * ((float)(totalTime - 70200) / 16200));
			else
				brightness = (maxBrightness - ((float)(totalTime - 27000) / 43200));

			return brightness;
		}

	}
}