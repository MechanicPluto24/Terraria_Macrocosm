using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.UI.LoadingScreens
{
	/// <summary> Title parameters. Used for loading screens. </summary>
	public struct TitleData
	{
		public string Text;
		public float Scale;
		public Color Color;
	}

	/// <summary> Loading screen, displayed when traveling to/from subworlds </summary>
    public abstract class LoadingScreen 
	{
		public static bool CurrentlyActive { get; set; }

		/// <summary> The title parameters </summary>
		public virtual TitleData Title { get; set; } = new TitleData()
		{
			Text = "",
			Scale = 1f,
			Color = Color.White
		};

		/// <summary> The current animation timer. Updated automatically, override <see cref="UpdateAnimation"/> for custom behavior </summary>
		protected double AnimationTimer = 0;

		/// <summary> The WorldGen progress bar, create instance with the desired parameters in the <see cref="LoadingScreen"/> constructor. </summary>
		protected UIWorldGenProgressBar ProgressBar { get; set; }

		private string message = "";

		/// <summary> Reset the animation timer. Useful when there's a persistent instance of a <see cref="LoadingScreen"/>. Call in the <see cref="Setup()"/> method. </summary>
		public void ResetAnimation() => AnimationTimer = 0;

		/// <summary> Setup tasks, called once before the <see cref="LoadingScreen"/> will be drawn. Useful when there's a persistent instance. </summary>
		public virtual void Setup() { }

		/// <summary> Used for miscellaneous update tasks </summary>
		public virtual void OnUpdate() { }

		/// <summary> Draw elements before the title, status messages, progress bar, etc. are drawn. </summary>
		public virtual void PreDraw(SpriteBatch spriteBatch) { }

		/// <summary> Draw elements after title, status messages, progress bar, etc. are drawn, but before the cursor is drawn and the spriteBatch is reset </summary>
		public virtual void PostDraw(SpriteBatch spriteBatch) { }

		private void InternalUpdate()
		{
			CurrentlyActive = true;

			if (ProgressBar is not null && WorldGenerator.CurrentGenerationProgress is not null)
				ProgressBar.SetProgress(WorldGenerator.CurrentGenerationProgress.TotalProgress, WorldGenerator.CurrentGenerationProgress.Value);

			Main.gameTips.Update();

			UpdateAnimation();
			OnUpdate();
		}

		/// <summary> Update the animation counter. Override for non-default behaviour. </summary>
		public virtual void UpdateAnimation()
		{
			if (AnimationTimer <= 5)
				AnimationTimer += 0.125;
		}

		/// <summary> Draws the loading screen. </summary>
		public void Draw(SpriteBatch spriteBatch) 
		{
			InternalUpdate();

			PreDraw(spriteBatch);

			if(WorldGenerator.CurrentGenerationProgress is not null)
			{
				message = WorldGenerator.CurrentGenerationProgress.Message;

				if(ProgressBar is not null)
				{
					ProgressBar.SetPosition((Main.screenWidth - ProgressBar.Width.Pixels) / 2, (Main.screenHeight - ProgressBar.Height.Pixels) / 2);
					ProgressBar.Draw(spriteBatch);
				}
			}
			else
			{
				message = Main.statusText;
			}

			Vector2 titleSize = FontAssets.DeathText.Value.MeasureString(Title.Text) * Title.Scale;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, Title.Text, new Vector2(Main.screenWidth / 2f - titleSize.X / 2f, titleSize.Y), Title.Color, 0f, Vector2.Zero, new Vector2(Title.Scale));
			
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, message, new Vector2(Main.screenWidth, Main.screenHeight - 100f) / 2f - FontAssets.DeathText.Value.MeasureString(message) / 2f, Color.White, 0f, Vector2.Zero, Vector2.One);
			Main.gameTips.Draw();

			PostDraw(spriteBatch);
 		}
	}
}
