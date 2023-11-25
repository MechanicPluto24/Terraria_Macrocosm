using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Drawing
{
    public class FadeEffect : ModSystem
    {
        public static bool IsFading => isFading;

        public static float CurrentFade => fadeAlpha / 255f;

        private static int fadeAlpha;
        private static float fadeSpeed;
        private static bool isFading;
        private static bool isFadingIn;
        private static bool interfaceSelfDraw;
        private static bool keepActiveUntilReset;

        public static void Draw()
        {
            if (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient)
                UpdateFadeEffect();

            DrawBlack(1f - fadeAlpha / 255f);
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (interfaceSelfDraw && (Main.hasFocus || Main.netMode == NetmodeID.MultiplayerClient))
            {
                if (isFading)
                {
                    Draw();
                }
                else
                {
                    if (!keepActiveUntilReset)
                    {
                        interfaceSelfDraw = false;
                        isFading = false;
                    }
                }
            }
        }

        public static void DrawBlack(float opacity)
        {
            Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1), Color.Black * opacity);
        }

        public static void ResetFade()
        {
            fadeAlpha = 0;
            isFading = false;
            isFadingIn = false;
            interfaceSelfDraw = false;
            keepActiveUntilReset = false;
        }

        public static void StartFadeIn(float speed = 0.098f, bool selfDraw = false, bool keepActive = false)
        {
            interfaceSelfDraw = selfDraw;
            keepActiveUntilReset = keepActive;
            fadeAlpha = 0;
            fadeSpeed = speed;
            isFadingIn = true;
            isFading = true;
        }

        public static void StartFadeOut(float speed = 0.098f, bool selfDraw = false, bool keepActive = false)
        {
            interfaceSelfDraw = selfDraw;
            keepActiveUntilReset = keepActive;
            fadeAlpha = 255;
            fadeSpeed = speed;
            isFadingIn = false;
            isFading = true;
        }

        private static void UpdateFadeEffect()
        {
            if (!isFading)
                return;

            if (isFadingIn)
            {
                fadeAlpha += (int)(fadeSpeed * 255f);
                if (fadeAlpha >= 255)
                {
                    fadeAlpha = 255;

                    if (!keepActiveUntilReset)
                        isFading = false;
                }
            }
            else
            {
                fadeAlpha -= (int)(fadeSpeed * 255f);
                if (fadeAlpha <= 0)
                {
                    fadeAlpha = 0;

                    if (!keepActiveUntilReset)
                        isFading = false;
                }
            }
        }
    }
}
