using Macrocosm.Common.Drawing;
using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.UI
{
	public static class LoadingTitleSequence
	{
		enum TitleState
		{
			FadingToBlack,
			Black,
			FadingTitleIn,
			TitleShown,
			FadingOut
		}

		public static LocalizedColorScaleText Title { get; set; }

		private static TitleState currentState = TitleState.FadingToBlack;
		private static float timer = 0;
		private static float titleFadeValue = 0f;   
		private static float titleFadeRate = 0.01f;

		public static void SetTargetWorld(string targetWorld)
		{
			switch (targetWorld)
			{
				case "Moon":
					Title = new(Language.GetText("Mods.Macrocosm.Subworlds.Moon.DisplayName"), Color.White, 1.2f, largeText: true);
					break;

				case "Earth":
					Title = new(Language.GetText("Mods.Macrocosm.Subworlds.Earth.DisplayName"), new Color(94, 150, 255), 1.2f, largeText: true);
					break;
			}
		}

		public static void StartSequence()
		{
			currentState = TitleState.FadingToBlack;
			timer = 0;
			titleFadeValue = 0f;
		}

		public static void Update(float elapsedSeconds)
		{
			switch (currentState)
			{
				case TitleState.FadingToBlack:
					if (!FadeEffect.IsFading && !Main.gameMenu)
					{
						FadeEffect.StartFadeOut(0.01f);
					}

					if (!FadeEffect.IsFading)
					{
						currentState = TitleState.Black;
						timer = 0;
					}
					break;

				case TitleState.Black:
					timer += elapsedSeconds;
					if (timer >= 1.5f)
					{
						currentState = TitleState.FadingTitleIn;
					}
					break;

				case TitleState.FadingTitleIn:
					titleFadeValue += titleFadeRate;   
					if (titleFadeValue >= 1f)   
					{
						titleFadeValue = 1f;
						currentState = TitleState.TitleShown;
						timer = 0;
					}
					break;

				case TitleState.TitleShown:
					timer += elapsedSeconds;
					if (timer >= 1.5f)
					{
						currentState = TitleState.FadingOut;
						FadeEffect.StartFadeIn(0.01f);
					}
					break;

				case TitleState.FadingOut:
					titleFadeValue -= titleFadeRate;   
					if (titleFadeValue <= 0f)   
					{
						titleFadeValue = 0f;
					}
					if (!FadeEffect.IsFading && titleFadeValue == 0f)
					{
						currentState = TitleState.FadingToBlack;   
					}
					break;
			}


		}

		public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (Main.hasFocus)
				Update((float)gameTime.ElapsedGameTime.TotalSeconds);

			switch (currentState)
			{
				case TitleState.FadingToBlack:
					MacrocosmSubworld.LoadingScreen.Draw(gameTime, spriteBatch);
					break;

				case TitleState.Black:
				case TitleState.TitleShown:
					FadeEffect.DrawBlack(1f);
					break;

				case TitleState.FadingTitleIn:
				case TitleState.FadingOut:
					FadeEffect.DrawBlack(FadeEffect.CurrentFade);
					Title?.DrawDirect(spriteBatch, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f), Title.Color * titleFadeValue);
					break;
			}
		}
	}
}
