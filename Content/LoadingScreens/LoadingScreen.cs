using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.LoadingScreens.WorldGen;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

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
        protected Stars fallingStars = new();

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
            stars = new(600, 700, wrapMode: MacrocosmStar.WrapMode.Random);
            fallingStars = new(15, 20, celledSpawn: true, wrapMode: MacrocosmStar.WrapMode.Random, falling: true, baseScale: 0.5f);

            fallingStars.Cast<FallingStar>().ToList().ForEach(star => star.Fall(deviationX: 0.1f, minSpeedY: 30f, maxSpeedY: 45f));

            Reset();
        }

        public void SetRocket(Rocket rocket)
        {
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
                stars.MovementVector = new(0f, 0.25f);
            }

            stars.Draw(spriteBatch);

            PreDraw(spriteBatch);

            if (Moving)
            {
                DrawRocket(spriteBatch);
                fallingStars.Draw(spriteBatch);
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

        /*
        private MethodInfo preDrawMenuMethodInfo;
        private MethodInfo drawMenuMethodInfo;
        private MethodInfo postDrawMenuMethodInfo;
        private void DrawVanillaMenu(GameTime gameTime)
        {
            preDrawMenuMethodInfo ??= typeof(Main).GetMethod("PreDrawMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            drawMenuMethodInfo ??= typeof(Main).GetMethod("DrawMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            postDrawMenuMethodInfo ??= typeof(Main).GetMethod("PostDrawMenu", BindingFlags.NonPublic | BindingFlags.Static);

            Main.spriteBatch.End();

            // PreDrawMenu(out var screenSizeCache, out var screenSizeCacheAfterScaling);
            object[] parameters = new object[2]; // 'PreDrawMenu' has two out parameters
            preDrawMenuMethodInfo.Invoke(Main.instance, parameters);
            Point screenSizeCache = (Point)parameters[0];
            Point screenSizeCacheAfterScaling = (Point)parameters[1];

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            // TODO: Replace with the provided parameter
            // Temporary fix until a future SubworldLibrary update
            // May break some mods that use gameTime in their DrawMenu
            gameTime = new GameTime();

            // DrawMenu(gameTime);
            drawMenuMethodInfo.Invoke(Main.instance, [gameTime]);

            // PostDrawMenu(screenSizeCache, screenSizeCacheAfterScaling);
            postDrawMenuMethodInfo.Invoke(null, [screenSizeCache, screenSizeCacheAfterScaling]);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
        }
        */
    }
}
