using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.UI.LoadingScreens
{
	public class MoonLoadingScreen : LoadingScreen
	{
		private Texture2D lunaBackground;
		private StarsDrawing starsDrawing;
		private CelestialBody earth;

		public MoonLoadingScreen()
		{
			lunaBackground = ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Luna", AssetRequestMode.ImmediateLoad).Value;		
			starsDrawing = new();

			Texture2D earthSmallBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/Earth", AssetRequestMode.ImmediateLoad).Value;
			Texture2D earthSmallAtmoBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthAtmo", AssetRequestMode.ImmediateLoad).Value;
			earth = new CelestialBody(earthSmallBackground, earthSmallAtmoBackground, 0.7f);

			ProgressBar = new(
			   ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/WorldGen/ProgressBarMoon", AssetRequestMode.ImmediateLoad).Value,
			   ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/WorldGen/ProgressBarMoon_Lower", AssetRequestMode.ImmediateLoad).Value,
			   new Color(56, 10, 28), new Color(155, 38, 74), new Color(6, 53, 27), new Color(93, 228, 162)
		   );
		}

		public override LocalizedColorScaleText Title => new(Language.GetText("Mods.Macrocosm.Subworlds.Moon.DisplayName"), Color.White, 1.2f, largeText: true);

		public override void Setup()
		{
			ResetAnimation();

			starsDrawing.Clear();
			starsDrawing.SpawnStars(200, 250, twinkleFactor: 0.1f);
		}

		private SpriteBatchState state;
		public override void PreDraw(SpriteBatch spriteBatch)
		{
			/*
			earth.SetPosition(Main.screenWidth * 0.265f, Main.screenHeight * 0.185f);

			Color bodyColor = (Color.White * (float)(AnimationTimer / 5) * 1f).WithOpacity(1f);

			state.SaveState(spriteBatch);
			spriteBatch.End();

			spriteBatch.Begin(BlendState.AlphaBlend, state);
			starsDrawing.Draw(spriteBatch);
			spriteBatch.End();

			float scale = Main.UIScale;

			spriteBatch.Begin(BlendState.NonPremultiplied, state);
			spriteBatch.Draw(
					lunaBackground,
					new Rectangle((int)(Main.screenWidth - lunaBackground.Width * scale),(int)(Main.screenHeight - lunaBackground.Height * scale + 50 - (int)(AnimationTimer * 10)), (int)(lunaBackground.Width * scale), (int)(lunaBackground.Height * scale)),
					null,
					bodyColor
			);

			earth.Scale = scale;
			earth.Draw(spriteBatch);

			spriteBatch.End();
			spriteBatch.Begin(state);
			*/
		}
	}
}
