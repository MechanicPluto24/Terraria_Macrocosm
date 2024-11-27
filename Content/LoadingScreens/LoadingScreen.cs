using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens.WorldGen;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;

namespace Macrocosm.Content.LoadingScreens
{
    /// <summary> Loading screen, displayed when traveling to/from subworlds. </summary>
    public abstract class LoadingScreen
    {
        /// <summary> Whether there is any type of LoadingScreen active right now. </summary>
        public static bool CurrentlyActive { get; set; }

        /// <summary> The current animation timer. Updated automatically, override <see cref="UpdateAnimation"/> for custom behavior </summary>
        protected float animationTimer = 0;

        protected Stars stars = new();

        protected bool Moving => rocket is not null;
        protected Rocket rocket;

        protected UIWorldGenProgressBar progressBar;

        protected string statusText;

        /// <summary> Reset the animation timer. Useful when there's a persistent instance of a <see cref="LoadingScreen"/>. Call in the <see cref="Reset()"/> method. </summary>
        public void ResetAnimation()
        {
            animationTimer = 0;
            FadeEffect.ResetFade();
            FadeEffect.StartFadeIn(0.012f);
        }

        public void SetProgressBar(UIWorldGenProgressBar progressBar) => this.progressBar = progressBar;

        public void Setup()
        {
            if (Moving)
            {
                // 3 screens horizontally
                // 5 screens vertically
                Rectangle area = new(
                    -Main.screenWidth,
                    -3 * Main.screenHeight,
                    3 * Main.screenWidth,
                    5 * Main.screenHeight
                );

                stars = new(8000, area, randomColor: true);
            }
            else
            {
                stars = new(600, randomColor: true);
            }

            Reset();
        }

        public void SetRocket(Rocket rocket)
        {
            if (rocket is null)
            {
                this.rocket = null;
                return;
            }

            var visualClone = rocket.VisualClone();
            visualClone.ForcedFlightAppearance = true;
            this.rocket = visualClone;
        }

        public void ClearRocket() => rocket = null;

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

            if (Moving && Main.rand.NextBool())
            {
                var fallingStars = stars.ToList();
                if (fallingStars.Count > 0)
                    fallingStars.GetRandom().Fall(deviationX: 0.1f, minSpeedY: 30f, maxSpeedY: 45f);
            }
        }

        /// <summary> Update the animation counter. Override for non-default behaviour </summary>
        protected virtual void UpdateAnimation()
        {
            if (!Moving && animationTimer > 5)
                return;

            if (animationTimer <= 5)
                animationTimer += 0.125f;
        }


        /// <summary> Draw elements before the title, status messages, progress bar, etc. are drawn, but after the common background elements are drawn </summary>
        protected virtual void PreDraw(SpriteBatch spriteBatch) { }

        /// <summary> Draw elements after title, status messages, progress bar, etc. are drawn, but before the cursor is drawn and the spriteBatch is reset </summary>
        protected virtual void PostDraw(SpriteBatch spriteBatch) { }


        /// <summary> Draws the loading screen. </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool drawStatusText = true)
        {
            InternalUpdate();

            if (Moving)
            {
                stars.CommonVelocity = new(0f, 0.25f);
            }

            stars.DrawStationary(spriteBatch);

            PreDraw(spriteBatch);

            if (Moving)
            {
                DrawRocket(spriteBatch);
                stars.DrawFalling(spriteBatch);
            }

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
            Vector2 center = Utility.ScreenCenter;
            Vector2 spriteSize = rocket.Bounds.Size();
            Vector2 randomOffset = Main.rand.NextVector2Circular(1f, 5f);

            Vector2 position = center - spriteSize * 0.5f + randomOffset;

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(state, Main.UIScaleMatrix);

            rocket.Draw(Rocket.DrawMode.Dummy, spriteBatch, position, useRenderTarget: false);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
