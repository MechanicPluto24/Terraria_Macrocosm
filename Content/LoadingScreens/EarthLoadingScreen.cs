using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.LoadingScreens
{
    public class EarthLoadingScreen : LoadingScreen
    {
        private static List<Asset<Texture2D>> earthBackgrounds;
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

            earthBackgrounds ??= [
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/Earth_Africa"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/Earth_Asia"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/Earth_Australia"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/Earth_Europe"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/Earth_NorthAmerica"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "OrbitBackgrounds/Earth_SouthAmerica")
            ];
        }

        private SpriteBatchState state;
        protected override void PreDraw(SpriteBatch spriteBatch)
        {
            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.NonPremultiplied, state);

            Color bodyColor = (Color.White * (float)(animationTimer / 5) * 1f).WithOpacity(1f);

            float progress = MathHelper.Clamp(animationTimer / animationDuration, 0f, 1f);
            progress = (float)Math.Pow(progress, 0.6);
            int movement = 250 + (int)(Utility.QuadraticEaseIn(progress) * 500f) * MovementDirection;

            Asset<Texture2D> earthBackground = earthBackgrounds[Utility.RealTimeCycle(earthBackgrounds.Count, 1800)];
            spriteBatch.Draw
            (
                earthBackground.Value,
                new Rectangle(0, movement, Main.screenWidth, Main.screenHeight),
                null,
                bodyColor
            );

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}
