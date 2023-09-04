using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.UI
{
	/// <summary> Loading screen, displayed when traveling to/from subworlds. </summary>
	public abstract class LoadingScreen 
    {
		/// <summary> Whether there is any type of LoadingScreen active right now. </summary>
		public static bool CurrentlyActive { get; set; }

		/// <summary> The current animation timer. Updated automatically, override <see cref="UpdateAnimation"/> for custom behavior </summary>
		protected float animationTimer = 0;

		protected Stars stars = new();
		protected Stars fallingStars = new();

		private Rocket rocket;

		private UIWorldGenProgressBar progressBar;

        private string statusText;

        /// <summary> Reset the animation timer. Useful when there's a persistent instance of a <see cref="LoadingScreen"/>. Call in the <see cref="Reset()"/> method. </summary>
        public void ResetAnimation() 
        {
            animationTimer = 0;
            FadeEffect.StartFadeIn(0.012f);
		}

        public void Setup()
        {
            stars = new(600, 700, wrapMode: MacrocosmStar.WrapMode.Random);
			fallingStars = new(15, 20, celledSpawn: true, wrapMode: MacrocosmStar.WrapMode.Random, falling: true, baseScale: 0.5f);

			fallingStars.Cast<FallingStar>().ToList().ForEach(star => star.Fall(deviationX: 0.1f, minSpeedY: 30f, maxSpeedY: 45f));

			Reset();
		}

        public void SetTargetWorld(string targetWorld)
        {
			switch (targetWorld)
			{
				case "Moon":
					progressBar = new(
						ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/WorldGen/ProgressBarMoon", AssetRequestMode.ImmediateLoad).Value,
						ModContent.Request<Texture2D>("Macrocosm/Content/UI/LoadingScreens/WorldGen/ProgressBarMoon_Lower", AssetRequestMode.ImmediateLoad).Value,
						new Color(56, 10, 28), new Color(155, 38, 74), new Color(6, 53, 27), new Color(93, 228, 162)
					);
                    break;

                case "Earth":
					break;
			}
		}

		public void SetRocket(Rocket rocket)
		{
			var visualClone = rocket.Clone();
			visualClone.ForcedFlightAppearance = true;
			this.rocket = visualClone;
		}

        /// <summary> Reset the loading screen specific variables. Called once before the <see cref="LoadingScreen"/> will be drawn. Useful when there's a persistent instance. </summary>
        protected virtual void Reset() { }

		/// <summary> Used for miscellaneous update tasks </summary>
		protected virtual void Update() { }

        private void InternalUpdate()
        {
            CurrentlyActive = true;

            if (progressBar is not null && WorldGenerator.CurrentGenerationProgress is not null)
                progressBar.SetProgress((float)WorldGenerator.CurrentGenerationProgress.TotalProgress, (float)WorldGenerator.CurrentGenerationProgress.Value);

            Main.gameTips.Update();

            UpdateAnimation();
            Update();
        }

        /// <summary> Update the animation counter. Override for non-default behaviour </summary>
        protected virtual void UpdateAnimation()
        {
            if (animationTimer <= 5)
                animationTimer += 0.125f;
        }


		/// <summary> Draw elements before the title, status messages, progress bar, etc. are drawn, but after the common background elements are drawn </summary>
		protected virtual void PreDraw(SpriteBatch spriteBatch) { }

		/// <summary> Draw elements after title, status messages, progress bar, etc. are drawn, but before the cursor is drawn and the spriteBatch is reset </summary>
		protected virtual void PostDraw(SpriteBatch spriteBatch) { }


		/// <summary> Draws the loading screen. </summary>
		public void Draw(GameTime gametime, SpriteBatch spriteBatch, bool drawStatusText = true)
        {
            InternalUpdate();

            stars.MovementVector = new(0f, 0.25f);
            stars.Draw(spriteBatch);

			PreDraw(spriteBatch);

            if (rocket is not null)
                DrawRocket(spriteBatch);

			fallingStars.Draw(spriteBatch);

			if (WorldGenerator.CurrentGenerationProgress is not null)
            {
				statusText = WorldGenerator.CurrentGenerationProgress.Message;

                if (progressBar is not null)
                {
                    progressBar.SetPosition(
                        (int)((Main.screenWidth - progressBar.Width.Pixels) / 2f),
                        (int)((Main.screenHeight - progressBar.Height.Pixels) / 2f)
                    );
                    progressBar.Draw(spriteBatch);
                }
            }
            else
            {
				statusText = Main.statusText;
            }

			if (drawStatusText)
			{
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, statusText, new Vector2(Main.screenWidth, Main.screenHeight - 100f) / 2f - FontAssets.DeathText.Value.MeasureString(statusText) / 2f, Color.White, 0f, Vector2.Zero, Vector2.One);
				Main.gameTips.Draw();
			}
			
			PostDraw(spriteBatch);

            FadeEffect.Draw();
		}

		private SpriteBatchState state;
		private void DrawRocket(SpriteBatch spriteBatch)
		{
			float scale = 1.4f;
			Vector2 center = Utility.ScreenCenter;
			Vector2 spriteSize = rocket.Bounds.Size();
			Vector2 randomOffset = Main.rand.NextVector2Circular(1f, 5f);

			// Use the position directly without scaling offset
			Vector2 position = center - spriteSize * 0.5f + randomOffset;

			Matrix transform =
				Matrix.CreateTranslation(-center.X, -center.Y, 0) *
				Matrix.CreateScale(scale, scale, 1f) *
				Matrix.CreateTranslation(center.X, center.Y, 0);

			state.SaveState(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(state, transform);
			rocket.DrawDummy(spriteBatch, position, Color.White);
			spriteBatch.End();

			spriteBatch.Begin(state);
		}
	}
}
