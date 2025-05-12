using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Macrocosm.Common.UI.Themes
{
    public readonly struct UIButtonStyle
    {
        /// <summary> The background color </summary>
        public Color BackgroundColor { get; } = Color.White;

        /// <summary> The border color </summary>
        public Color BorderColor { get; } = new Color(85, 80, 85);

        /// <summary> The highlighted background color </summary>
        public Color BackgroundColorHighlight { get; } = Color.White;

        /// <summary> The highlighted border color </summary>
        public Color BorderColorHighlight { get; } = Color.Gold;

        public UIButtonStyle() { }
        public UIButtonStyle(Color color, Color borderColor, Color highlightColor, Color borderHighlightColor)
        {
            BackgroundColor = color;
            BorderColor = borderColor;
            BackgroundColorHighlight = highlightColor;
            BorderColorHighlight = borderHighlightColor;
        }
    }
}
