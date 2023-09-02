using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.Drawing
{
	public class FadeEffect : ModSystem
	{
		public static bool IsFading => isFading;

		public static float CurrentFade => fadeAlpha / 255f;

		private static int fadeAlpha;
		private static float fadeSpeed;
		private static bool isFading;
		private static bool isFadingIn;
		private static bool selfDraw;

		public static void Draw()
		{
			if(Main.hasFocus)
				UpdateFadeEffect();

			DrawBlack(1f - fadeAlpha / 255f);
		}

		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
			base.PostDrawInterface(spriteBatch);
		}

		public static void DrawBlack(float opacity)
		{
			Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * opacity);
		}

		public static void StartFadeIn(float speed = 0.098f, bool selfDraw = false)
		{
			fadeAlpha = 0;
			fadeSpeed = speed;
			isFadingIn = true;
			isFading = true;
		}

		public static void StartFadeOut(float speed = 0.098f, bool selfDraw = false)
		{
			fadeAlpha = 255;
			fadeSpeed = speed;
			isFadingIn = false;
			isFading = true;
		}

		private static void UpdateFadeEffect()
		{
			if (!isFading)
				return;

			if (isFadingIn)
			{
				fadeAlpha += (int)(fadeSpeed * 255f);
				if (fadeAlpha >= 255)
				{
					fadeAlpha = 255;
					isFading = false;
				}
			}
			else
			{
				fadeAlpha -= (int)(fadeSpeed * 255f);
				if (fadeAlpha <= 0)
				{
					fadeAlpha = 0;
					isFading = false;
				}
			}
		}
	}
}
