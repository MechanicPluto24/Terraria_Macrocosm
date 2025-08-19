using Macrocosm.Common.Config;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;

namespace Macrocosm.Common.UI.Themes;

public readonly partial struct UITheme
{
    public static readonly UITheme Terraria = new(nameof(Terraria))
    {
        PanelStyle = new UIPanelStyle(),
        WindowStyle = new UIPanelStyle(new Color(33, 43, 79) * 0.8f, Color.Black),
        InfoElementStyle = new UIPanelStyle(new Color(57, 74, 136), Color.Black),
        PanelButtonStyle = new UIButtonStyle(new Color(73, 85, 186), Color.Black, new Color(73, 85, 186), Color.Gold),
        IconButtonStyle = new UIButtonStyle(),
        InventorySlotStyle = new UIPanelStyle(new Color(220, 220, 220, 220), default),
        ScrollbarStyle = new UIScrollbarStyle(),
        SeparatorColor = new Color(89, 116, 213),
        CommonTextColor = Color.White
    };

    public static readonly UITheme Macrocosm = new(nameof(Macrocosm))
    {
        PanelStyle = new UIPanelStyle(new Color(53, 72, 135), new Color(89, 116, 213)),
        WindowStyle = new UIPanelStyle(new Color(58, 81, 166) * 0.8f, new Color(15, 15, 15)),
        InfoElementStyle = new UIPanelStyle(new Color(43, 56, 101), new Color(86, 112, 202)),
        PanelButtonStyle = new UIButtonStyle(new Color(43, 56, 101), new Color(86, 112, 202), new Color(43, 56, 101), Color.Gold),
        IconButtonStyle = new UIButtonStyle(new Color(86, 112, 202), new Color(43, 56, 101), new Color(60, 78, 141), Color.Gold),
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

