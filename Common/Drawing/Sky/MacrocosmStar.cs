using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing.Sky
{
	public class MacrocosmStar : Star
    {
        public float Brightness;

        private Texture2D texture;
        private Color color;
        private Color newColor;
        private bool colorOverridden = false;

        /// <summary>
        /// Adapted from Star.SpawnStars
        /// </summary>
        /// <param name="baseScale"> The average scaling of the stars relative to vanilla </param>
        /// <param name="twinkleFactor"> How much a star will twinkle, keep between (0f, 1f); 0.4f for vanilla effect</param>
        public MacrocosmStar(float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            FastRandom fastRandom = FastRandom.CreateWithRandomSeed();

            texture = TextureAssets.Star[fastRandom.Next(0, 4)].Value;
            Brightness = 1f;
            color = Color.White;

            position.X = fastRandom.Next(Main.screenWidth + 1);
            position.Y = fastRandom.Next(Main.screenHeight + 1);
            rotation = fastRandom.Next(628) * 0.01f;
            scale = fastRandom.Next(70, 130) * 0.006f * baseScale;
            twinkle = Math.Clamp(fastRandom.Next(1, 101) * 0.01f, 1f - twinkleFactor, 1f);
            twinkleSpeed = fastRandom.Next(30, 110) * 0.0001f; // TODO: add constructor argument for this

            if (fastRandom.Next(2) == 0)
                twinkleSpeed *= -1f;

            rotationSpeed = fastRandom.Next(5, 50) * 0.0001f;
            if (fastRandom.Next(2) == 0)
                rotationSpeed *= -1f;

            if (fastRandom.Next(40) == 0)
            {
                scale *= 2f * (0.6f + twinkleFactor);
                twinkleSpeed /= 2f;
                rotationSpeed /= 2f;
            }
        }

        public MacrocosmStar(Vector2 position, float baseScale = 1f, float twinkleFactor = 0.4f) : this(baseScale, twinkleFactor)
        {
            this.position = position;
        }

        public MacrocosmStar(Vector2 position, Texture2D tex, float baseScale = 1f, float twinkleFactor = 0.4f) : this(position, twinkleFactor, baseScale)
        {
            texture = tex;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, color, rotation, texture.Size() / 2, scale * twinkle, default, 0f);
        }

        public new void Update()
        {
            base.Update();
            TwinkleColor();
        }

        public void OverrideColor(float r, float g, float b) => OverrideColor(new Color(r, g, b));
        public void OverrideColor(Color color)
        {
            newColor = color;
            newColor.A = this.color.A;
            colorOverridden = true;
        }

        /// <summary>
        /// Adapted from Main.DrawStarsInBackround
        /// </summary>
        private void TwinkleColor()
        {

            int red;
            int green;
            int blue;
            float fade = 1f - fadeIn;

            if (!colorOverridden)
            {
                red = (int)(155 * twinkle * fade);
                green = (int)(155 * twinkle * fade);
                blue = (int)(155 * twinkle * fade);

                red = (red + green + blue) / 3;

                if (red <= 0)
                    return;

                red = (int)(red * 1.4);

                if (red > 255)
                    red = 255;

                green = red;
                blue = red;
            }
            else
            {
                red = newColor.R;
                green = newColor.G;
                blue = newColor.B;
            }

            color.R = (byte)red;
            color.G = (byte)green;
            color.B = (byte)blue;

            color *= Brightness;
        }
    }
}
