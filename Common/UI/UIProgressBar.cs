using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIProgressBar : UIElement
    {
        private readonly Asset<Texture2D> border;       
        private readonly Asset<Texture2D> background;    
        private readonly Asset<Texture2D> fill;          

        private float progress;
        public float Progress
        {
            get => progress;
            set
            {
                progress = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        public Color TextureColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Gray;
        public Color FillColor { get; set; } = Color.White;

        public UIProgressBar(Asset<Texture2D> border, Asset<Texture2D> background, Asset<Texture2D> fill)
        {
            this.border = border;
            this.background = background;
            this.fill = fill;

            Width.Set(border.Width(), 0f);
            Height.Set(border.Height(), 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle dimensions = GetDimensions();

            spriteBatch.Draw(border.Value, dimensions.Position(), TextureColor);
            spriteBatch.Draw(background.Value, dimensions.Position(), BackgroundColor);

            int fillWidth = (int)(fill.Width() * progress);
            if (fillWidth > 0)
            {
                Rectangle fillSourceRectangle = new(0, 0, fillWidth, fill.Height());
                Vector2 fillPosition = dimensions.Position();
                spriteBatch.Draw(fill.Value, fillPosition, fillSourceRectangle, FillColor);
            }
        }
    }
}
