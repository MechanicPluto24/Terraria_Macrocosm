using Microsoft.Xna.Framework;

namespace Macrocosm.Common.UI.Themes
{
    public readonly struct UIScrollbarStyle
    {
        public Color Fill1Color { get; } = new Color(73, 94, 171);
        public Color Fill2Color { get; } = new Color(43, 56, 101);
        public Color BorderColor { get; } = Color.Black;
        public Color InnerColor { get; } = Color.White;
        public Color InnerBorderColor { get; } = new Color(85, 80, 85);

        public UIScrollbarStyle()
        {
        }

        public UIScrollbarStyle(Color fill1Color, Color fill2Color, Color borderColor, Color innerColor, Color innerBorderColor)
        {
            Fill1Color = fill1Color;
            Fill2Color = fill2Color;
            BorderColor = borderColor;
            InnerColor = innerColor;
            InnerBorderColor = innerBorderColor;
        }
    }
}
