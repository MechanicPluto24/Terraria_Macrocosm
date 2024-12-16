using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.LoadingScreens
{
    public class MoonLoadingScreen : LoadingScreen
    {
        private static Asset<Texture2D> lunaBackground;
        private CelestialBody earth;

        private readonly float animationDuration = 1000f;
        protected override void UpdateAnimation()
        {
            if (!Moving && animationTimer > 5)
                return;

            if (animationTimer < animationDuration)
                animationTimer += 1f;
        }

        protected override void Reset()
        {
            ResetAnimation();

            lunaBackground ??= ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/Luna");

            Asset<Texture2D> earthSmallBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Skies/Moon/Earth", AssetRequestMode.ImmediateLoad);
            Asset<Texture2D> earthSmallAtmoBackground = ModContent.Request<Texture2D>("Macrocosm/Content/Skies/Moon/EarthAtmo", AssetRequestMode.ImmediateLoad);
            earth ??= new CelestialBody(earthSmallBackground, earthSmallAtmoBackground, scale: 0.7f);
        }

        private SpriteBatchState state;
        protected override void PreDraw(SpriteBatch spriteBatch)
        {
            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.NonPremultiplied, state);

            Color bodyColor = (Color.White * (float)(animationTimer / 5) * 1f).WithOpacity(1f);
            float scale = Main.UIScale * 0.75f;

            float progress = MathHelper.Clamp(animationTimer / animationDuration, 0f, 1f);
            progress = (float)Math.Pow(progress, 0.6);
            int movement = 500 + (int)(Utility.QuadraticEaseIn(progress) * 500f) * MovementDirection;

            spriteBatch.Draw(
                    lunaBackground.Value,
                    new Rectangle((int)(Main.screenWidth - lunaBackground.Width() * scale), (int)(Main.screenHeight - lunaBackground.Height() * scale + movement), (int)(lunaBackground.Width() * scale), (int)(lunaBackground.Height() * scale)),
                    null,
                    bodyColor
            );

            progress = MathHelper.Clamp(animationTimer / animationDuration, 0f, 1f);
            progress = (float)Math.Pow(progress, 0.2);
            movement = (int)(Utility.QuadraticEaseIn(progress) * 100f) * MovementDirection;

            earth.Color = bodyColor;
            earth.Scale = scale;
            earth.SetPosition(Main.screenWidth * 0.265f, Main.screenHeight * 0.185f + movement);
            earth.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
