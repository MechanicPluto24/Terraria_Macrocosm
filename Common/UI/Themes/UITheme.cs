using Microsoft.Xna.Framework;

namespace Macrocosm.Common.UI.Themes;

/// <summary> The theme used by this mod's UI. Defauls to the Terraria style </summary>
public readonly partial struct UITheme
{
    public string Name { get; init; } = "default";

    // TODO
    /// <summary> Currently only partially used </summary>
    public Color CommonTextColor { get; init; } = Color.White;

    public Color SeparatorColor { get; init; } = new Color(89, 116, 213);

    public UIPanelStyle PanelStyle { get; init; } = new UIPanelStyle();
    public UIPanelStyle WindowStyle { get; init; } = new UIPanelStyle(new Color(33, 43, 79) * 0.8f, Color.Black);

    public UIPanelStyle InfoElementStyle { get; init; } = new UIPanelStyle(new Color(57, 74, 136), Color.Black);
    public UIButtonStyle PanelButtonStyle { get; init; } = new UIButtonStyle(new Color(73, 85, 186), Color.Black, new Color(73, 85, 186), Color.Gold);
    public UIButtonStyle IconButtonStyle { get; init; } = new UIButtonStyle();
    public UIPanelStyle InventorySlotStyle { get; init; } = new UIPanelStyle(new Color(220, 220, 220, 220), default);

    public UIScrollbarStyle ScrollbarStyle { get; init; } = new UIScrollbarStyle();

    public UITheme() { }
    public UITheme(string name)
    {
        Name = name;
    }
}
