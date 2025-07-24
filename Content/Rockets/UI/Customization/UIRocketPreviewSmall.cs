using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.UI.Customization;

public class UIRocketPreviewSmall : UIPanel, IRocketUIDataConsumer
{
    public Rocket Rocket { get; set; }

    private Rocket visualClone;

    private UIText uITitle;

    public UIRocketPreviewSmall()
    {
    }

    public override void OnInitialize()
    {
        Width.Set(0, 0.124f);
        Height.Set(0, 0.336f);
        HAlign = 0f;
        Top.Set(0, 0.01f);
        Left.Set(0, 0.01f);
        BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
        BorderColor = UITheme.Current.PanelStyle.BorderColor;

        uITitle = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization"), 0.8f, false)
        {
            IsWrapped = false,
            HAlign = 0.5f,
            VAlign = 0.005f,
            TextColor = Color.White
        };

        Append(uITitle);
    }

    public void OnTabOpen()
    {
        if (Rocket is not null)
            GetClone();
    }

    private void GetClone()
    {
        visualClone = Rocket.VisualClone();
        visualClone.ForcedStationaryAppearance = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (IsMouseHovering)
        {
            BorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor;
            BackgroundColor = UITheme.Current.ButtonHighlightStyle.BackgroundColor;
        }
        else
        {
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
        }
    }

    private SpriteBatchState state;
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (Rocket is null)
            return;

        if (visualClone is null)
            GetClone();

        RenderTarget2D renderTarget = visualClone.GetRenderTarget(Rocket.DrawMode.Dummy);
        Rectangle uiRect = GetDimensions().ToRectangle();
        int horizontalPadding = 0;
        int verticalPadding = 0;
        Rectangle rect = new(
            uiRect.X + horizontalPadding,
            uiRect.Y + verticalPadding,
            uiRect.Width - 2 * horizontalPadding,
            uiRect.Height - 2 * verticalPadding
        );

        float rtAspect = (float)renderTarget.Width / renderTarget.Height;
        float availableAspect = (float)rect.Width / rect.Height;
        int finalWidth, finalHeight;
        if (rtAspect > availableAspect)
        {
            finalWidth = rect.Width;
            finalHeight = (int)(rect.Width / rtAspect);
        }
        else
        {
            finalHeight = rect.Height;
            finalWidth = (int)(rect.Height * rtAspect);
        }
        int finalX = rect.X + (rect.Width - finalWidth) / 2;
        int finalY = rect.Y + (rect.Height - finalHeight) / 2 + (int)uITitle.GetDimensions().Height;
        Rectangle finalRect = new(finalX, finalY, finalWidth, finalHeight);

        int targetPixelsX = 48;
        int targetPixelsY = (int)(targetPixelsX / rtAspect);

        Effect effect = Macrocosm.GetShader("Pixelate");
        effect.Parameters["uPixelCount"].SetValue(new Vector2(targetPixelsX, targetPixelsY));

        state.SaveState(spriteBatch);
        spriteBatch.EndIfBeginCalled();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, state.RasterizerState, effect, Main.UIScaleMatrix);

        spriteBatch.Draw(renderTarget, finalRect, Color.White);

        spriteBatch.End();
        spriteBatch.Begin(state);
    }

}
