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
using SubworldLibrary;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.LoadingScreens
{
    // TODO:
    // Zoom reset causes:
    //		- some stars to align on the edge due to their movement vector and wrapping mechanic,
    //		- offset the rocket trail.
    // What's weird is that it only happens sometimes. 
    // Disabling SubLib's Zoom reset on Subworld.DrawSetup seems to NOT fix it.
    // Pls help :sadcat: -- Feldy

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

        private UIWorldGenProgressBar progressBar;

        private string statusText;

        /// <summary> Reset the animation timer. Useful when there's a persistent instance of a <see cref="LoadingScreen"/>. Call in the <see cref="Reset()"/> method. </summary>
        public void ResetAnimation()
        {
            animationTimer = 0;
            FadeEffect.ResetFade();
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
                        ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/ProgressBarMoon", AssetRequestMode.ImmediateLoad).Value,
                        ModContent.Request<Texture2D>("Macrocosm/Content/LoadingScreens/WorldGen/ProgressBarMoon_Lower", AssetRequestMode.ImmediateLoad).Value,
                        new Color(56, 10, 28), new Color(155, 38, 74), new Color(6, 53, 27), new Color(93, 228, 162)
                    );
                    break;

                case "Earth":
                    break;
            }
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

            // Make SubworldSystem.cache null if on game menu. 
            // Remove once https://github.com/jjohnsnaill/SubworldLibrary/pull/35 is merged.
            if (Main.gameMenu && Main.menuMode == 0)
                 MacrocosmSubworld.Hacks.SubworldSystem_NullCache();
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
        public void Draw(GameTime gametime, SpriteBatch spriteBatch, bool drawStatusText = true)
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
            spriteBatch.Begin(state, Main.GameViewMatrix.ZoomMatrix);

            rocket.DrawDummy(spriteBatch, position, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
