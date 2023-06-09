using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Utils.IO;
using Macrocosm.Content.UI.LoadingScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.UI.LoadingScreens
{
    public class EarthLoadingScreen : LoadingScreen
	{
		private List<Texture2D> earthBackgrounds;
		private Texture2D earthBackground;

		private StarsDrawing starsDrawing;

		public EarthLoadingScreen()
		{
			AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

			earthBackgrounds = new List<Texture2D>(){
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Africa").Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Asia", mode).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Australia", mode).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Europe", mode).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_NorthAmerica", mode).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_SouthAmerica", mode).Value
			};

			starsDrawing = new();
		}

		public override TitleData Title => new()
		{
			TextKey = "Earth", //FIXME: localize
			Scale = 1.2f,
			Color = new Color(94, 150, 255)
		};

		public override void Setup()
		{
			ResetAnimation();

			earthBackground = earthBackgrounds.GetRandom();

			starsDrawing.Clear();
 			starsDrawing.SpawnStars(150, 200);
		}

		public override void PreDraw(SpriteBatch spriteBatch)
		{
			Color bodyColor = (Color.White * (float)(AnimationTimer / 5) * 1f).NewAlpha(1f);

			var state = spriteBatch.SaveState();

			spriteBatch.End();

			spriteBatch.Begin(BlendState.AlphaBlend, state);
			starsDrawing.Draw(spriteBatch);
			spriteBatch.End();


			spriteBatch.Begin(BlendState.NonPremultiplied, state);
			spriteBatch.Draw
			(
				earthBackground,
				//new Rectangle((int)(Main.screenWidth - earthBackground.Width * scale),(int)(Main.screenHeight - earthBackground.Height * scale + 50 - (int)(AnimationTimer * 10)), (int)(earthBackground.Width * scale), (int)(earthBackground.Height * scale)),
				new Rectangle(0, 50 - (int)(AnimationTimer * 10), Main.screenWidth, Main.screenHeight),
				null,
				bodyColor
			);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}
}
