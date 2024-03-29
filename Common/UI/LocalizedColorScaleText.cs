using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Macrocosm.Common.UI
{
    /// <summary> Contains data about drawn text, including its LocalizedText reference and appearance </summary>
    public class LocalizedColorScaleText
    {
        public LocalizedText LocalizedText;
        public Color Color;
        public float Scale;

        public bool LargeText = false;

        /// <summary> Create a new LocalizedColorScaleText. You may get the desired LocalizedText with <see cref="Language.GetText(string)"/> </summary>
        /// <param name="text"> The localized text </param>
        /// <param name="color"> The text color </param>
        /// <param name="scale"> The text scale </param>
        /// <param name="largeText"> Whether to use the large text sprite font </param>
        public LocalizedColorScaleText(LocalizedText text, Color color = default, float scale = 1f, bool largeText = false)
        {
            LocalizedText = text;
            Color = color == default ? Color.White : color;
            Scale = scale;
            LargeText = largeText;
        }

        /// <summary> Gets a <see cref="UIText"/> based on the text data </summary>
        public UIText ProvideUI()
        {
            return new(LocalizedText, Scale, LargeText)
            {
                TextColor = Color,
            };
        }

        /// <summary> Draws the text directly to the screen </summary>
        public void DrawDirect(SpriteBatch spriteBatch, Vector2 centerPosition, Color? overrideColor = null)
        {
            DynamicSpriteFont font = LargeText ? FontAssets.DeathText.Value : FontAssets.MouseText.Value;
            Utility.DrawString(spriteBatch, font, LocalizedText.Value, centerPosition - (font.MeasureString(LocalizedText.Value) * Scale / 2f), overrideColor is null ? Color : overrideColor.Value, 0f, Vector2.Zero, new Vector2(Scale));
        }
    }
}
