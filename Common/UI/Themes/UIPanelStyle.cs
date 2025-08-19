using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Macrocosm.Common.UI.Themes;

public readonly struct UIPanelStyle
{
    /// <summary> The background color </summary>
    public Color BackgroundColor { get; init; } = new Color(73, 94, 171);

    /// <summary> The border color </summary>
    public Color BorderColor { get; init; } = Color.Black;

    public UIPanelStyle() { }
    public UIPanelStyle(Color backgroundColor, Color borderColor)
    {
        BackgroundColor = backgroundColor;
        BorderColor = borderColor;
    }

    // TODO
    /// <summary> Currently unused </summary>
    public Asset<Texture2D> BackgroundTexture { get; init; } = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");

    // TODO
    /// <summary> Currently unused </summary>
    public Asset<Texture2D> BorderTexture { get; init; } = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
}
