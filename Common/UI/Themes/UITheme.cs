using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Macrocosm.Common.UI.Themes
{
    /// <summary> The theme used by this mod's UI. Defauls to the default Macrocosm style </summary>
    public readonly partial struct UITheme
    {
        public readonly struct UIPanelStyle
        {
            /// <summary> The background color </summary>
            public Color BackgroundColor { get; init; }

            /// <summary> The border color </summary>
            public Color BorderColor { get; init; }

            // TODO
            /// <summary> Currently unused </summary>
            public Asset<Texture2D> BackgroundTexture { get; init; } = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");

            // TODO
            /// <summary> Currently unused </summary>
            public Asset<Texture2D> BorderTexture { get; init; } = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");

            public UIPanelStyle(Color backgroundColor, Color borderColor)
            {
                BackgroundColor = backgroundColor;
                BorderColor = borderColor;
            }
        }

        public string Name { get; init; }

        // TODO
        /// <summary> Currently only partially used </summary>
        public Color CommonTextColor { get; init; } = Color.White;

        public Color SeparatorColor { get; init; } = new(89, 116, 213);

        public UIPanelStyle PanelStyle { get; init; } = new(new(53, 72, 135), new(89, 116, 213));
        public UIPanelStyle TabStyle { get; init; } = new(new(58, 81, 166), new(15, 15, 15));
        public UIPanelStyle WindowStyle { get; init; } = new(new(89, 116, 213), new(15, 15, 15));

        public UIPanelStyle InfoElementStyle { get; init; } = new(new(43, 56, 101), new(86, 112, 202));
        public UIPanelStyle ButtonStyle { get; init; } = new(new(43, 56, 101), new(86, 112, 202));
        public UIPanelStyle ButtonHighlightStyle { get; init; } = new(new(60, 78, 141), Color.Gold);
        public UIPanelStyle InventorySlotStyle { get; init; } = new(new(45, 62, 115), new(89, 116, 213));

        public UITheme(string name)
        {
            Name = name;
        }
    }
}
