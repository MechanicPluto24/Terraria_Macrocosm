using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UITextureProgressBar : UIElement
    {
        private readonly Asset<Texture2D> border;
        private readonly Asset<Texture2D> background;
        private readonly Asset<Texture2D> fill;

        private float progress;
        public float Progress
        {
            get => progress;
            set => progress = MathHelper.Clamp(value, 0f, 1f);
        }

        public bool IsVertical { get; set; } = false;

        public Color BorderColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Gray;

        public List<Color> FillColors { get; set; } = new List<Color> { Color.White };

        public UITextureProgressBar(Asset<Texture2D> border, Asset<Texture2D> background, Asset<Texture2D> fill)
        {
            this.border = border;
            this.background = background;
            this.fill = fill;

            if (IsVertical)
            {
                Width.Set(border.Height(), 0f);
                Height.Set(border.Width(), 0f);
            }
            else
            {
                Width.Set(border.Width(), 0f);
                Height.Set(border.Height(), 0f);
            }
        }

        private Color GetInterpolatedColor(float progress)
        {
            if (FillColors.Count == 1)
                return FillColors[0];

            float scaledProgress = progress * (FillColors.Count - 1);
            int index = (int)scaledProgress;
            float remainder = scaledProgress - index;

            Color startColor = FillColors[index];
            Color endColor = FillColors[(int)MathHelper.Clamp(index + 1, 0, FillColors.Count - 1)];

            return Color.Lerp(startColor, endColor, remainder);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position();

            if (!IsVertical)
            {
                spriteBatch.Draw(border.Value, position, null, BorderColor);
                spriteBatch.Draw(background.Value, position, null, BackgroundColor);

                int fillWidth = (int)(fill.Width() * progress);
                if (fillWidth > 0)
                {
                    // Apply gradient across the width of the bar
                    for (int x = 0; x < fillWidth; x++)
                    {
                        float progressAtX = (float)x / fillWidth;
                        Color fillColorAtX = GetInterpolatedColor(progressAtX);

                        Rectangle slice = new(x, 0, 1, fill.Height());
                        Vector2 slicePosition = new(position.X + x, position.Y);

                        spriteBatch.Draw(fill.Value, slicePosition, slice, fillColorAtX);
                    }
                }
            }
            else
            {
                float rotation = MathHelper.PiOver2;
                Vector2 origin = Vector2.Zero;

                Vector2 borderPosition = new(position.X + border.Height(), position.Y);
                Vector2 backgroundPosition = new(position.X + background.Height(), position.Y);

                spriteBatch.Draw(border.Value, borderPosition, null, BorderColor, rotation, origin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(background.Value, backgroundPosition, null, BackgroundColor, rotation, origin, 1f, SpriteEffects.None, 0f);

                int fillHeight = (int)(fill.Height() * progress);
                if (fillHeight > 0)
                {
                    for (int y = 0; y < fillHeight; y++)
                    {
                        float progressAtY = (float)y / fillHeight;
                        Color fillColorAtY = GetInterpolatedColor(progressAtY);

                        Rectangle slice = new(0, fill.Height() - fillHeight + y, fill.Width(), 1);
                        Vector2 slicePosition = new(position.X + fill.Height(), position.Y + (fill.Height() - fillHeight + y));

                        spriteBatch.Draw(fill.Value, slicePosition, slice, fillColorAtY, rotation, origin, 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
