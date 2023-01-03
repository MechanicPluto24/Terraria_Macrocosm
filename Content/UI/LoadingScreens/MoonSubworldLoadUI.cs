using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Utils.IO;
using Macrocosm.Content.UI.WorldGen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.UI.LoadingScreens
{
    public class MoonSubworldLoadUI : UIWorldLoad
	{
		private Texture2D lunaBackground;
		private Texture2D earthBackground;

		private bool toEarth;

		private double animationTimer;
		private string chosenMessage;

		private MacrocosmUIGenProgressBar progressBar;
		private CelestialBody earth;

		private StarsDrawing starsDrawing = new();
		private readonly TextFileLoader textFileLoader = new();

		public void ResetAnimation() => animationTimer = 0;
		public void NewStatusMessage() => chosenMessage = toEarth ? ListRandom.Pick(textFileLoader.Parse("Content/Subworlds/Messages/EarthMessages")) : ListRandom.Pick(textFileLoader.Parse("Content/Subworlds/Messages/MoonMessages"));

		public MoonSubworldLoadUI()
		{
			lunaBackground = ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Luna", AssetRequestMode.ImmediateLoad).Value;
			earthBackground = ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/Backgrounds/Earth", AssetRequestMode.ImmediateLoad).Value;

			Texture2D earthSmallBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/Earth", AssetRequestMode.ImmediateLoad).Value;
			Texture2D earthSmallAtmoBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Backgrounds/Moon/EarthAtmo", AssetRequestMode.ImmediateLoad).Value;

			Texture2D moonProgressBarTexUpper = ModContent.Request<Texture2D>("Macrocosm/Content/UI/WorldGen/ProgressBarMoon", AssetRequestMode.ImmediateLoad).Value;
			Texture2D moonProgressBarTexLower = ModContent.Request<Texture2D>("Macrocosm/Content/UI/WorldGen/ProgressBarMoon_Lower", AssetRequestMode.ImmediateLoad).Value;

			earth = new CelestialBody(earthSmallBackground, earthSmallAtmoBackground, 0.7f);

			progressBar = new MacrocosmUIGenProgressBar(moonProgressBarTexUpper, moonProgressBarTexLower, new Color(56, 10, 28), new Color(155, 38, 74), new Color(6, 53, 27), new Color(93, 228, 162));
			progressBar.SetPosition(200f, 200f);
		}

		public void Setup(bool toEarth)
		{
			this.toEarth = toEarth;

			ResetAnimation();
			NewStatusMessage();
			starsDrawing.Clear();

			if (toEarth)
				starsDrawing.SpawnStars(150, 200);
			else
				starsDrawing.SpawnStars(200, 250, twinkleFactor: 0.1f);
		}

		public new void DrawSelf(SpriteBatch spriteBatch)
		{
			string loadText;

			Color bodyColor = Color.White * (float)(animationTimer / 5) * 1f;  // color of the celestial body
			bodyColor.A = byte.MaxValue;                                       // keep it opaque

			spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

			if (toEarth)
			{
				starsDrawing.Draw(spriteBatch);

				spriteBatch.EndIfBeginCalled();
				spriteBatch.Begin(0, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw
				(
					earthBackground,
					new Rectangle(Main.screenWidth - earthBackground.Width, Main.screenHeight - earthBackground.Height + 50 - (int)(animationTimer * 10), earthBackground.Width, earthBackground.Height),
					null,
					bodyColor
				);
				string msgToPlayer = "Earth"; // Title
				Vector2 messageSize = FontAssets.DeathText.Value.MeasureString(msgToPlayer) * 1.2f;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), new Color(94, 150, 255), 0f, Vector2.Zero, new Vector2(1.2f));
				Vector2 messageSize2 = FontAssets.DeathText.Value.MeasureString(chosenMessage) * 0.7f;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));

				spriteBatch.End();
				spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
			}
			else
			{
				earth.SetPosition(510, 200);
				earth.Draw(spriteBatch);

				starsDrawing.Draw(spriteBatch);

				spriteBatch.EndIfBeginCalled();
				spriteBatch.Begin(0, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
				spriteBatch.Draw
				(
					lunaBackground,
					new Rectangle(Main.screenWidth - lunaBackground.Width, Main.screenHeight - lunaBackground.Height + 50 - (int)(animationTimer * 10), lunaBackground.Width, lunaBackground.Height),
					null,
					bodyColor
				);

				string msgToPlayer = "Earth's Moon"; // Title
				Vector2 messageSize = FontAssets.DeathText.Value.MeasureString(msgToPlayer) * 1.2f;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), Color.White, 0f, Vector2.Zero, new Vector2(1.2f));
				Vector2 messageSize2 = FontAssets.DeathText.Value.MeasureString(chosenMessage) * 0.7f;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));
				spriteBatch.End();
				spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
			}

			if (WorldGenerator.CurrentGenerationProgress != null)
			{
				loadText = WorldGenerator.CurrentGenerationProgress.Message;
				progressBar.SetProgress(WorldGenerator.CurrentGenerationProgress.TotalProgress, WorldGenerator.CurrentGenerationProgress.Value);
				progressBar.SetPosition((Main.screenWidth - progressBar.Width.Pixels) / 2, (Main.screenHeight - progressBar.Height.Pixels) / 2);
				progressBar.DrawSelf(spriteBatch);
			}
			else
			{
				loadText = Main.statusText;
			}

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, loadText, new Vector2(Main.screenWidth, Main.screenHeight - 100f) / 2f - FontAssets.DeathText.Value.MeasureString(loadText) / 2f, Color.White, 0f, Vector2.Zero, Vector2.One);

			Main.DrawCursor(Main.DrawThickCursor(false), false);

			spriteBatch.End();

			animationTimer += 0.125;
			if (animationTimer > 5)
				animationTimer = 5;
		}
	}
}
