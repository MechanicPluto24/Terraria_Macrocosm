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
			earthBackgrounds = new List<Texture2D>(){
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Africa", AssetRequestMode.ImmediateLoad).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Asia", AssetRequestMode.ImmediateLoad).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Australia", AssetRequestMode.ImmediateLoad).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_Europe", AssetRequestMode.ImmediateLoad).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_NorthAmerica", AssetRequestMode.ImmediateLoad).Value,
				ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth_SouthAmerica", AssetRequestMode.ImmediateLoad).Value
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
				new Rectangle(Main.screenWidth - earthBackground.Width, Main.screenHeight - earthBackground.Height + 50 - (int)(AnimationTimer * 10), earthBackground.Width, earthBackground.Height),
				null,
				bodyColor
			);
			
			spriteBatch.Restore(state);
		}
	}
}
