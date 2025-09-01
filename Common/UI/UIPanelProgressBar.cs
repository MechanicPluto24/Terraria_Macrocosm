using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace Macrocosm.Common.UI;

public class UIPanelProgressBar : UIElement
{
    private readonly int cornerSize;
    private readonly int barSize;
    private readonly Asset<Texture2D> borderTexture;
    private readonly Asset<Texture2D> backgroundTexture;
    private readonly Asset<Texture2D> fillTexture;

    public float Progress
    {
        get => progress;
        set => progress = MathHelper.Clamp(value, 0f, 1f);
    }
    private float progress;

    public bool IsVertical { get; set; } = false;

    public Color BackgroundColor { get; set; } = new Color(63, 82, 151) * 0.7f;
    public Color FillColor { get; set; } = Color.White;
    public Color FillColorEnd { get; set; } = Color.White;
    public Color BorderColor { get; set; } = Color.Black;

    public UIPanelProgressBar()
    {
        cornerSize = 12;
        barSize = 4;
        SetPadding(cornerSize);

        borderTexture = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
        backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
        fillTexture = backgroundTexture;
    }

    public UIPanelProgressBar(Asset<Texture2D> fillTexture = null, Asset<Texture2D> backgroundTexture = null, Asset<Texture2D> borderTexture = null, int customCornerSize = 12, int customBarSize = 4)
    {
        cornerSize = customCornerSize;
        barSize = customBarSize;
        SetPadding(cornerSize);

        this.borderTexture = borderTexture ?? Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
        this.backgroundTexture = backgroundTexture ?? Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
        this.fillTexture = fillTexture ?? backgroundTexture;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        DrawPanel(spriteBatch, backgroundTexture.Value, BackgroundColor);

        DrawFill(spriteBatch);

        DrawPanel(spriteBatch, borderTexture.Value, BorderColor);
    }

    private void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Color color)
    {
        CalculatedStyle dimensions = GetDimensions();
        Rectangle panelArea = new((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height);

        int left = panelArea.Left;
        int right = panelArea.Right;
        int top = panelArea.Top;
        int bottom = panelArea.Bottom;

        spriteBatch.Draw(texture, new Rectangle(left, top, cornerSize, cornerSize), new Rectangle(0, 0, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(right - cornerSize, top, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, 0, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(left, bottom - cornerSize, cornerSize, cornerSize), new Rectangle(0, cornerSize + barSize, cornerSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(right - cornerSize, bottom - cornerSize, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, cornerSize + barSize, cornerSize, cornerSize), color);

        spriteBatch.Draw(texture, new Rectangle(left + cornerSize, top, panelArea.Width - 2 * cornerSize, cornerSize), new Rectangle(cornerSize, 0, barSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(left + cornerSize, bottom - cornerSize, panelArea.Width - 2 * cornerSize, cornerSize), new Rectangle(cornerSize, cornerSize + barSize, barSize, cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(left, top + cornerSize, cornerSize, panelArea.Height - 2 * cornerSize), new Rectangle(0, cornerSize, cornerSize, barSize), color);
        spriteBatch.Draw(texture, new Rectangle(right - cornerSize, top + cornerSize, cornerSize, panelArea.Height - 2 * cornerSize), new Rectangle(cornerSize + barSize, cornerSize, cornerSize, barSize), color);

        spriteBatch.Draw(texture, new Rectangle(left + cornerSize, top + cornerSize, panelArea.Width - 2 * cornerSize, panelArea.Height - 2 * cornerSize), new Rectangle(cornerSize, cornerSize, barSize, barSize), color);
    }

    private SpriteBatchState state;
    private void DrawFill(SpriteBatch spriteBatch)
    {
        CalculatedStyle dimensions = GetDimensions();
        Rectangle panelArea = new Rectangle((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height);

        // Calculate fill area based on progress
        Rectangle fillArea;
        if (!IsVertical)
        {
            int fillWidth = (int)(panelArea.Width * Progress);
            fillArea = new Rectangle((int)dimensions.X, (int)dimensions.Y, fillWidth, (int)dimensions.Height);
        }
        else
        {
            int fillHeight = (int)(panelArea.Height * Progress);
            fillArea = new Rectangle((int)dimensions.X, (int)(dimensions.Y + dimensions.Height - fillHeight), (int)dimensions.Width, fillHeight);
        }

        if (fillArea.Width <= 0 || fillArea.Height <= 0)
            return;

        RasterizerState previousRasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
        Rectangle previousScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

        RasterizerState rasterizerState = new()
        {
            ScissorTestEnable = true
        };

        state.SaveState(spriteBatch);
        spriteBatch.End();

        Matrix transformMatrix = state.Matrix;
        Rectangle scissorRectangle = TransformRectangle(fillArea, transformMatrix);
        Rectangle viewportBounds = spriteBatch.GraphicsDevice.Viewport.Bounds;
        scissorRectangle = Rectangle.Intersect(viewportBounds, scissorRectangle);
        spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

        Effect effect = Macrocosm.GetShader("Gradient");
        effect.Parameters["uHorizontal"].SetValue(!IsVertical);
        effect.Parameters["uColorStart"].SetValue(FillColor.ToVector4());
        effect.Parameters["uColorEnd"].SetValue(FillColorEnd.ToVector4());

        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            state.BlendState,
            state.SamplerState,
            state.DepthStencilState,
            rasterizerState,
            effect,
            transformMatrix
        );

        DrawPanel(spriteBatch, fillTexture.Value, FillColor);

        spriteBatch.End();
        spriteBatch.GraphicsDevice.ScissorRectangle = previousScissorRectangle;
        spriteBatch.GraphicsDevice.RasterizerState = previousRasterizerState;

        spriteBatch.Begin(state);
    }

    private Rectangle TransformRectangle(Rectangle rectangle, Matrix transform)
    {
        Vector2 topLeft = Vector2.Transform(new Vector2(rectangle.Left, rectangle.Top), transform);
        Vector2 bottomRight = Vector2.Transform(new Vector2(rectangle.Right, rectangle.Bottom), transform);

        int x = (int)topLeft.X;
        int y = (int)topLeft.Y;
        int width = (int)(bottomRight.X - topLeft.X);
        int height = (int)(bottomRight.Y - topLeft.Y);

        return new Rectangle(x, y, width, height);
    }
}