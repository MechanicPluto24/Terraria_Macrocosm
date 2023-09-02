using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing
{
	public class GlobalVFX : ModSystem
	{
		
		#region Celestial Disco
		public enum CelestialType { Nebula, Stardust, Vortex, Solar }
		public static CelestialType CelestialStyle { get; set; } = CelestialType.Nebula;
		public static CelestialType NextCelestialStyle
			=> CelestialStyle == CelestialType.Solar ? CelestialType.Nebula : CelestialStyle + 1;

		public static float CelestialStyleProgress;
		private static int celesialCounter = 0;

		public static Color NebulaColor { get; set; } = new(165, 0, 204);
		public static Color StardustColor { get; set; } = new(0, 187, 255);
		public static Color VortexColor { get; set; } = new(0, 255, 180);
		public static Color SolarColor { get; set; } = new(255, 191, 0);

		public static Color CelestialColor { get; set; }

		private static readonly Color[] celestialColors = { NebulaColor, StardustColor, VortexColor, SolarColor };
		#endregion

		#region Fade Effect
		private static int fadeAlpha;
		private static float fadeSpeed;
		private static bool isFading;
		private static bool isFadingIn;
		#endregion

		public override void PostUpdateEverything()
		{
			UpdateCelestialStyle();
		}

		private static void UpdateCelestialStyle()
		{
			float cyclePeriod = 90f;
			if (celesialCounter++ >= (int)cyclePeriod)
			{
				celesialCounter = 0;
				CelestialStyle = NextCelestialStyle;
			}

			CelestialStyleProgress = celesialCounter / cyclePeriod;

			CelestialColor = Color.Lerp(celestialColors[(int)CelestialStyle], celestialColors[(int)NextCelestialStyle], CelestialStyleProgress);
		}

		private static void UpdateFadeEffect()
		{
			if (!isFading)
				return;

			if (isFadingIn)
			{
				fadeAlpha += (int)(fadeSpeed * 255); 
				if (fadeAlpha >= 255)
				{
					fadeAlpha = 255;
					isFading = false;
				}
			}
			else
			{
				fadeAlpha -= (int)(fadeSpeed * 255);
				if (fadeAlpha <= 0)
				{
					fadeAlpha = 0;
					isFading = false;
				}
			}
		}

		public static void DrawFade()
		{
			UpdateFadeEffect();
			Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height), Color.Black * (1f - fadeAlpha / 255f));
		}

		public static void StartFadeIn(float speed = 0.098f)
		{
			fadeAlpha = 0;
			fadeSpeed = speed;
			isFadingIn = true;
			isFading = true;
		}

		public static void StartFadeOut(float speed = 0.098f)
		{
			fadeAlpha = 255;
			fadeSpeed = speed;
			isFadingIn = false;
			isFading = true;
		}
	}
}
