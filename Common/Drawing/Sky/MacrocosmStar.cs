using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Macrocosm.Common.Drawing.Sky
{
    /// <summary> Represents a sky star. Adapted from Terraria.Star </summary>
    public class MacrocosmStar
    {
        public float Scale { get; set; }
        public float Brightness { get; set; }
        public Vector2 Velocity { get; set; }
        public bool Falling => falling;

        protected Vector2 position;
        protected float rotation;
        protected float rotationSpeed;

        protected float twinkle;
        protected float twinkleSpeed;

        protected readonly Asset<Texture2D> texture;
        protected Color color;
        protected Color newColor;
        protected bool colorOverridden = false;

        private bool falling = false;
        private Vector2 fallSpeed = Vector2.Zero;
        private float fallTime = 0.0f;

        /// <summary>
        /// Adapted from Star.SpawnStars
        /// </summary>
        /// <param name="baseScale"> The average scaling of the stars relative to vanilla </param>
        /// <param name="twinkleFactor"> How much a star will twinkle, keep between (0f, 1f); 0.4f for vanilla effect</param>
        public MacrocosmStar(Vector2 position, float baseScale = 1f, float twinkleFactor = 0.4f, Color? color = null)
        {
            FastRandom fastRandom = FastRandom.CreateWithRandomSeed();

            texture = TextureAssets.Star[fastRandom.Next(0, 4)];
            Brightness = 1f;

            this.color = Color.White;
            if (color.HasValue)
            {
                this.color = color.Value;
                OverrideColor(color.Value);
            }

            this.position = position;
            rotation = fastRandom.Next(628) * 0.01f;
            Scale = fastRandom.Next(70, 130) * 0.006f * baseScale;
            twinkle = Math.Clamp(fastRandom.Next(1, 101) * 0.01f, 1f - twinkleFactor, 1f);
            twinkleSpeed = fastRandom.Next(30, 110) * 0.0001f;

            if (fastRandom.Next(2) == 0)
                twinkleSpeed *= -1f;

            rotationSpeed = fastRandom.Next(5, 50) * 0.0001f;
            if (fastRandom.Next(2) == 0)
                rotationSpeed *= -1f;

            if (fastRandom.Next(40) == 0)
            {
                Scale *= 2f * (0.6f + twinkleFactor);
                twinkleSpeed /= 2f;
                rotationSpeed /= 2f;
            }
        }

        public MacrocosmStar(Vector2 position, Asset<Texture2D> texture, float baseScale = 1f, float twinkleFactor = 0.4f) : this(position, twinkleFactor, baseScale)
        {
            this.texture = texture;
        }

        public void OverrideColor(float r, float g, float b) => OverrideColor(new Color(r, g, b));
        public void OverrideColor(Color color)
        {
            newColor = color;
            newColor.A = this.color.A;
            colorOverridden = true;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset = default)
        {
            if(position.X > 0 && position.X < Main.screenWidth && position.Y > 0 && position.Y < Main.screenHeight)
            {
                if (falling)
                {
                    float maxTrailLength = 30f;
                    float trailLength = Math.Min(fallTime, maxTrailLength);

                    for (int j = 1; j < trailLength; j++)
                    {
                        Vector2 trailOffset = fallSpeed * j * 0.05f;
                        float trailScale = Scale * (1f - j / maxTrailLength);
                        Color trailColor = color * (1f - j / maxTrailLength);

                        spriteBatch.Draw(texture.Value, position - trailOffset, null, trailColor, rotation, texture.Size() / 2, trailScale * twinkle, default, 0f);
                    }
                }

                spriteBatch.Draw(texture.Value, position + offset, null, color, rotation, texture.Size() / 2, Scale * twinkle, default, 0f);
            }
        }

        public virtual void Update()
        {
            if (falling)
            {
                fallTime += (float)Main.desiredWorldEventsUpdateRate;
                Velocity = fallSpeed * (float)(Main.desiredWorldEventsUpdateRate + 99.0) / 100f;
            }

            position += Velocity;
            Twinkle();
        }

        // Adapted from Terraria.Star.Update and Terraria.Main.DrawStarsInBackround 
        protected virtual void Twinkle()
        {
            twinkle += twinkleSpeed;
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

            rotation += rotationSpeed;
            if (rotation > 6.28)
                rotation -= 6.28f;

            if (rotation < 0f)
                rotation += 6.28f;

            if (falling)
                return;

            //TODO: to add fade in/out mechanism
            float fade = 1f;

            int red, green, blue;
            if (!colorOverridden)
            {
                red = (int)(155 * twinkle * fade);
                green = (int)(155 * twinkle * fade);
                blue = (int)(155 * twinkle * fade);

                float avg = (red + green + blue) / 3;

                if (avg > 0f)
                    red = green = blue = (int)MathHelper.Clamp(avg * 1.4f, 0, 255);
            }
            else
            {
                red = newColor.R;
                green = newColor.G;
                blue = newColor.B;
            }

            color = new()
            {
                R = (byte)red,
                G = (byte)green,
                B = (byte)blue,
                A = 255
            };

            color *= Brightness;
        }

        public void Fall(float deviationX = 4f, float minSpeedY = 7, float maxSpeedY = 10)
        {
            fallTime = 0.0f;
            falling = true;
            fallSpeed.Y = Main.rand.NextFloat(minSpeedY, maxSpeedY);
            fallSpeed.X = Main.rand.NextFloat(-deviationX, deviationX);
        }
    }
}
