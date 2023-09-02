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

		private Vector2 position;
		private float rotation;
        private float rotationSpeed;

		private float twinkle;
        private float twinkleSpeed;

		private readonly Asset<Texture2D> texture;
        private Color color;
        private Color newColor;
        private bool colorOverridden = false;

		private Vector2 previousScreenSize;

		/// <summary>
		/// Adapted from Star.SpawnStars
		/// </summary>
		/// <param name="baseScale"> The average scaling of the stars relative to vanilla </param>
		/// <param name="twinkleFactor"> How much a star will twinkle, keep between (0f, 1f); 0.4f for vanilla effect</param>
		public MacrocosmStar(float baseScale = 1f, float twinkleFactor = 0.4f)
        {
            FastRandom fastRandom = FastRandom.CreateWithRandomSeed();

            texture = TextureAssets.Star[fastRandom.Next(0, 4)];
            Brightness = 1f;
            color = Color.White;

            position = new(fastRandom.Next(Main.screenWidth + 1), fastRandom.Next(Main.screenHeight + 1));
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

        public MacrocosmStar(Vector2 position, float baseScale = 1f, float twinkleFactor = 0.4f) : this(baseScale, twinkleFactor)
        {
            this.position = position;
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

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture.Value, position, null, color, rotation, texture.Size() / 2, Scale * twinkle, default, 0f);
		}

		public void Update()
        {
			OnScreenResizeReposition();
			Twinkle();
        }

        public void UpdatePosition(Vector2 delta, bool wrapOnScreen = true)
        {
            position += delta;

            if (wrapOnScreen && delta != Vector2.Zero)
            {
				if (position.X > Main.screenWidth)
					position.X = 0;
				else if (position.X < 0)
					position.X = Main.screenWidth;

				if (position.Y > Main.screenHeight)
					position.Y = 0;
				else if (position.Y < 0)
					position.Y = Main.screenHeight;
			}
		}

		private void OnScreenResizeReposition()
		{
			if (previousScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
			{
				if (previousScreenSize != Vector2.Zero)
					position = new(Main.rand.Next(Main.screenWidth + 1), Main.rand.Next(Main.screenHeight + 1));

				previousScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
			}
		}

        // Adapted from Terraria.Star.Update and Terraria.Main.DrawStarsInBackround 
        private void Twinkle()
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

            //TODO: to add fade in/out mechanism
            float fade = 1f;

			int red, green, blue;
            if (!colorOverridden)
            {
                red = (int)(155 * twinkle * fade);
                green = (int)(155 * twinkle * fade);
                blue = (int)(155 * twinkle * fade);

                float avg = (red + green + blue) / 3;

                if(avg > 0f)
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
    }
}
