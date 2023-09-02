using Macrocosm.Common.Drawing;
using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.UI
{
	public class LoadingTitleSequence : ModSystem
	{ 
		enum TitleState
		{
			Inactive,
			FadingToBlack,
			Black,
			FadingTitleIn,
			TitleShown,
			FadingOut
		}

		public static LocalizedColorScaleText Title { get; set; }

		private static TitleState currentState = TitleState.Inactive;
		private static float timer = 0;
		private static float titleFadeValue = 0f;   
		private static float titleFadeRate = 0.01f;

		public override void Load()
		{
			Player.Hooks.OnEnterWorld += OnEnterWorld;
		}

		public override void Unload()
		{
			Player.Hooks.OnEnterWorld -= OnEnterWorld;
			Title = null;
		}

		private void OnEnterWorld(Player player)
		{
			StartSequence();
		}

		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
			if (currentState != TitleState.Inactive)
			{
				if (Main.hasFocus)
					Update();

				Draw(spriteBatch);
			}
		}

		public static void SetTargetWorld(string targetWorld)
		{
			switch (targetWorld)
			{
				case "Moon":
					Title = new(Language.GetText("Mods.Macrocosm.Subworlds.Moon.DisplayName"), Color.White, 1.2f, largeText: true);
					break;

				case "Earth":
					Title = null;
					break;
			}
		}

		public static void StartSequence()
		{
			timer = 0;
			titleFadeValue = 0f;
			//FadeEffect.StartFadeIn(0.01f);
			currentState = TitleState.FadingToBlack;
		}

		public static void Update()
		{
			switch (currentState)
			{
				case TitleState.Inactive:
					break;

				case TitleState.FadingToBlack:
					if (!FadeEffect.IsFading)
					{
						currentState = TitleState.Black;
						timer = 0;
					}
					break;

				case TitleState.Black:
					timer++;
					if (timer >= 30) // 0.5 seconds
					{
						if(Title is null)
						{
							FadeEffect.StartFadeIn(0.01f);
							currentState = TitleState.FadingOut;
						}
						else
						{
							currentState = TitleState.FadingTitleIn;
						}

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
					timer++;
					if (timer >= 90) // 1.5 seconds
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
						currentState = TitleState.Inactive;   
					}
					break;
			}
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			switch (currentState)
			{
				case TitleState.FadingToBlack:
					//MacrocosmSubworld.LoadingScreen?.Draw(gameTime, spriteBatch);
					//FadeEffect.Draw();
					break;

				case TitleState.Black:
				case TitleState.FadingTitleIn:
				case TitleState.TitleShown:
					FadeEffect.DrawBlack(1f);
					Title?.DrawDirect(spriteBatch, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.2f), Title.Color * titleFadeValue);
					break;

				case TitleState.FadingOut:
					FadeEffect.Draw();
					Title?.DrawDirect(spriteBatch, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.2f), Title.Color * titleFadeValue);
					break;
			}
		}
	}
}
