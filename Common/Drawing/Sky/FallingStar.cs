using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace Macrocosm.Common.Drawing.Sky
{
    /// <summary> Falling "star". Adapted from Terraria.Star </summary>
    internal class FallingStar : MacrocosmStar
    {
        private float fallTime = 0.0f;
        private bool falling = false;
        private Vector2 fallSpeed = Vector2.Zero;

        public FallingStar(float baseScale = 1f, float twinkleFactor = 0.4f, WrapMode wrapMode = WrapMode.None) : base(baseScale, twinkleFactor, wrapMode) { }

        public FallingStar(Vector2 position, float baseScale = 1f, float twinkleFactor = 0.4f, WrapMode wrapMode = WrapMode.None) : base(position, baseScale, twinkleFactor, wrapMode) { }

        public FallingStar(Vector2 position, Asset<Texture2D> texture, float baseScale = 1f, float twinkleFactor = 0.4f, WrapMode wrapMode = WrapMode.None) : base(position, texture, baseScale, twinkleFactor, wrapMode) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            const float maxTrailLength = 30f;
            float trailLength = Math.Min(fallTime, maxTrailLength);

            for (int j = 1; j < trailLength; j++)
            {
                Vector2 trailOffset = fallSpeed * j * 0.05f;
                float trailScale = Scale * (1f - j / maxTrailLength);
                Color trailColor = color * (1f - j / maxTrailLength);

                spriteBatch.Draw(texture.Value, position - trailOffset, null, trailColor, rotation, texture.Size() / 2, trailScale * twinkle, default, 0f);
            }

            spriteBatch.Draw(texture.Value, position, null, color, rotation, texture.Size() / 2, Scale * twinkle, default, 0f);

        }

        public void Fall(float deviationX = 4f, float minSpeedY = 7, float maxSpeedY = 10)
        {
            fallTime = 0.0f;
            falling = true;
            fallSpeed.Y = Main.rand.NextFloat(minSpeedY, maxSpeedY);
            fallSpeed.X = Main.rand.NextFloat(-deviationX, deviationX);
        }

        public override void Update()
        {
            if (falling)
            {
                fallTime += (float)Main.desiredWorldEventsUpdateRate;
                UpdatePosition(fallSpeed * (float)(Main.desiredWorldEventsUpdateRate + 99.0) / 100f);
            }

            Twinkle();
        }

        private void Twinkle()
        {
            twinkle += twinkleSpeed * 3f;

            if (twinkle > 1f)
            {
                twinkle = 1f;
                twinkleSpeed *= -1f;
            }
            else if (twinkle < 0.6)
            {
                twinkle = 0.6f;
                twinkleSpeed *= -1f;
            }

            rotation += 0.5f;
            if (rotation > 6.28)
                rotation -= 6.28f;
            if (rotation < 0f)
                rotation += 6.28f;
        }
    }
}
