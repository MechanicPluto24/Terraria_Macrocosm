using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.UI
{
	/// <summary> Loading screen, displayed when traveling to/from subworlds. </summary>
	public abstract class LoadingScreen : ILoadable
    {
		public void Load(Mod mod)
		{
		}

		public void Unload()
		{
            Rocket = null;
		}

		/// <summary> Whether there is any type of LoadingScreen active right now. </summary>
		public static bool CurrentlyActive { get; set; }

        /// <summary> The title parameters </summary>
        public virtual LocalizedColorScaleText Title { get; set; } 

        protected string statusText;

        /// <summary> The current animation timer. Updated automatically, override <see cref="UpdateAnimation"/> for custom behavior </summary>
        protected double AnimationTimer = 0;

		private StarsDrawing starDrawing = new();

		/// <summary> The WorldGen progress bar, create instance with the desired parameters in the <see cref="LoadingScreen"/> constructor. </summary>
		protected UIWorldGenProgressBar ProgressBar { get; set; }

        /// <summary> Reset the animation timer. Useful when there's a persistent instance of a <see cref="LoadingScreen"/>. Call in the <see cref="Setup()"/> method. </summary>
        public void ResetAnimation() 
        {
            AnimationTimer = 0;
            fadeout = 0;
        }

        public void Setup1()
        {
            starDrawing = new();
			starDrawing.SpawnStars(600, 700);
            Setup();
		}

        /// <summary> Setup tasks, called once before the <see cref="LoadingScreen"/> will be drawn. Useful when there's a persistent instance. </summary>
        public virtual void Setup() { }

        /// <summary> Used for miscellaneous update tasks </summary>
        public virtual void Update() { }

        /// <summary> Draw elements before the title, status messages, progress bar, etc. are drawn. </summary>
        public virtual void PreDraw(SpriteBatch spriteBatch) { }

        /// <summary> Draw elements after title, status messages, progress bar, etc. are drawn, but before the cursor is drawn and the spriteBatch is reset </summary>
        public virtual void PostDraw(SpriteBatch spriteBatch) { }

        public static Rocket Rocket { get; set; }

        private void InternalUpdate()
        {
            CurrentlyActive = true;

            if (ProgressBar is not null && WorldGenerator.CurrentGenerationProgress is not null)
                ProgressBar.SetProgress((float)WorldGenerator.CurrentGenerationProgress.TotalProgress, (float)WorldGenerator.CurrentGenerationProgress.Value);

            Main.gameTips.Update();

            UpdateAnimation();
            Update();
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

            starDrawing.GlobalOffset = new(0f, 7f);
            starDrawing.Draw(spriteBatch);

			PreDraw(spriteBatch);

            if (Rocket is not null)
                DrawRocket(spriteBatch);

			if (WorldGenerator.CurrentGenerationProgress is not null)
            {
				statusText = WorldGenerator.CurrentGenerationProgress.Message;

                if (ProgressBar is not null)
                {
                    ProgressBar.SetPosition(
                        (int)((Main.screenWidth - ProgressBar.Width.Pixels) / 2f),
                        (int)((Main.screenHeight - ProgressBar.Height.Pixels) / 2f)
                    );
                    ProgressBar.Draw(spriteBatch);
                }
            }
            else
            {
				statusText = Main.statusText;
            }

            Title.DrawDirect(spriteBatch, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.1f));
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, statusText, new Vector2(Main.screenWidth, Main.screenHeight - 100f) / 2f - FontAssets.DeathText.Value.MeasureString(statusText) / 2f, Color.White, 0f, Vector2.Zero, Vector2.One);
			Main.gameTips.Draw();
			PostDraw(spriteBatch);

            DrawFadeout(spriteBatch);
		}

        private float fadeout;
        private void DrawFadeout(SpriteBatch spriteBatch)
        {
            if (fadeout < 0.98f)
                fadeout += 0.5f;
            else
                fadeout = 1f;

			spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.graphics.GraphicsDevice.Viewport.Width, Main.graphics.GraphicsDevice.Viewport.Height), Color.Black.WithOpacity(1f - fadeout));
		}

		private SpriteBatchState state;
		private void DrawRocket(SpriteBatch spriteBatch)
		{
			float scale = 1.4f;
			Vector2 center = Utility.ScreenCenter;
			Vector2 spriteSize = Rocket.Bounds.Size();
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
			Rocket.DrawDummy(spriteBatch, position, Color.White);
			spriteBatch.End();

			spriteBatch.Begin(state);
		}
	}
}
