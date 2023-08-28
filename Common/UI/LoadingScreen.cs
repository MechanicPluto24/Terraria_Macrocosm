using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS.Windows;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.UI
{
    /// <summary> Loading screen, displayed when traveling to/from subworlds. </summary>
    internal abstract class LoadingScreen
    {
        /// <summary> Whether there is any type of LoadingScreen active right now. </summary>
        public static bool CurrentlyActive { get; set; }

        /// <summary> The title parameters </summary>
        public virtual LocalizedColorScaleText Title { get; set; } 

        protected string statusText;

        /// <summary> The current animation timer. Updated automatically, override <see cref="UpdateAnimation"/> for custom behavior </summary>
        protected double AnimationTimer = 0;

        /// <summary> The WorldGen progress bar, create instance with the desired parameters in the <see cref="LoadingScreen"/> constructor. </summary>
        protected UIWorldGenProgressBar ProgressBar { get; set; }

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
                ProgressBar.SetProgress((float)WorldGenerator.CurrentGenerationProgress.TotalProgress, (float)WorldGenerator.CurrentGenerationProgress.Value);

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
		}
    }
}
