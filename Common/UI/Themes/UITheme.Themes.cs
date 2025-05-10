using Macrocosm.Common.Config;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;

namespace Macrocosm.Common.UI.Themes
{
    public readonly partial struct UITheme
    {
        public static readonly UITheme Terraria = new(nameof(Terraria))
        {
            PanelStyle = new UIPanelStyle(),
            TabStyle = new UIPanelStyle(Color.Transparent, Color.Transparent),
            WindowStyle = new UIPanelStyle(new Color(33, 43, 79) * 0.8f, Color.Black),
            InfoElementStyle = new UIPanelStyle(new Color(57, 74, 136), Color.Black),
            ButtonStyle = new UIPanelStyle(new Color(73, 85, 186), Color.Black),
            ButtonHighlightStyle = new UIPanelStyle(new Color(69, 90, 166), Color.Gold),
            InventorySlotStyle = new UIPanelStyle(new Color(220, 220, 220, 220), default),
            ScrollbarStyle = new UIScrollbarStyle(),
            SeparatorColor = new Color(89, 116, 213),
            CommonTextColor = Color.White
        };

        public static readonly UITheme Macrocosm = new(nameof(Macrocosm))
        {
            PanelStyle = new UIPanelStyle(new Color(53, 72, 135), new Color(89, 116, 213)),
            TabStyle = new UIPanelStyle(new Color(58, 81, 166), new Color(15, 15, 15)),
            WindowStyle = new UIPanelStyle(new Color(89, 116, 213), new Color(15, 15, 15)),
            InfoElementStyle = new UIPanelStyle(new Color(43, 56, 101), new Color(86, 112, 202)),
            ButtonStyle = new UIPanelStyle(new Color(43, 56, 101), new Color(86, 112, 202)),
            ButtonHighlightStyle = new UIPanelStyle(new Color(60, 78, 141), Color.Gold),
            InventorySlotStyle = new UIPanelStyle(new Color(45, 62, 115), new Color(89, 116, 213)),
            ScrollbarStyle = new UIScrollbarStyle(
                fill1Color: new Color(53, 72, 135),
                fill2Color: new Color(45, 62, 115),
                borderColor: new Color(89, 116, 213),
                innerColor: new Color(89, 116, 213),
                innerBorderColor:  new Color(43, 56, 101)
            ),
            SeparatorColor = new(89, 116, 213),
            CommonTextColor = Color.White
        };

    }
}

