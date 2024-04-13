using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Elements;

/// <summary>
/// Adaption of Terraria.GameContent.UI.Elements.UIText that allows snippets
/// </summary>
public class UISnippetText : UIElement
{
    private object text = "";
    private float textScale = 1f;
    private Vector2 textSize = Vector2.Zero;
    private bool isLarge;
    private Color color = Color.White;
    private Color shadowColor = Color.Black;
    private bool isWrapped;
    public bool DynamicallyScaleDownToWidth;
    private string visibleText;
    private string lastTextReference;

    public string Text => text.ToString();
    public float TextOriginX { get; set; }
    public float TextOriginY { get; set; }
    public float WrappedTextBottomPadding { get; set; }

    public bool AllowSnippets { get; set; } = true;

    public bool IsWrapped
    {
        get => isWrapped;
        set
        {
            isWrapped = value;
            if (value)
                MinWidth.Set(0, 0);
            InternalSetText(text, textScale, isLarge);
        }
    }

    public Color TextColor
    {
        get => color;
        set => color = value;
    }

    public Color ShadowColor
    {
        get => shadowColor;
        set => shadowColor = value;
    }

    public event Action OnInternalTextChange;

    public UISnippetText(string text, float textScale = 1f, bool large = false)
    {
        TextOriginX = 0.5f;
        TextOriginY = 0f;
        IsWrapped = false;
        WrappedTextBottomPadding = 20f;
        InternalSetText(text, textScale, large);
    }

    public UISnippetText(LocalizedText text, float textScale = 1f, bool large = false)
    {
        TextOriginX = 0.5f;
        TextOriginY = 0f;
        IsWrapped = false;
        WrappedTextBottomPadding = 20f;
        InternalSetText(text, textScale, large);
    }

    public override void Recalculate()
    {
        InternalSetText(text, textScale, isLarge);
        base.Recalculate();
    }

    public void SetText(string text)
    {
        InternalSetText(text, textScale, isLarge);
    }

    public void SetText(LocalizedText text)
    {
        InternalSetText(text, textScale, isLarge);
    }

    public void SetText(string text, float textScale, bool large)
    {
        InternalSetText(text, textScale, large);
    }

    public void SetText(LocalizedText text, float textScale, bool large)
    {
        InternalSetText(text, textScale, large);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        VerifyTextState();
        CalculatedStyle innerDimensions = GetInnerDimensions();
        Vector2 position = innerDimensions.Position();
        if (isLarge)
            position.Y -= 10f * textScale;
        else
            position.Y -= 2f * textScale;

        position.X += (innerDimensions.Width - textSize.X) * TextOriginX;
        position.Y += (innerDimensions.Height - textSize.Y) * TextOriginY;
        float scale = textScale;
        if (DynamicallyScaleDownToWidth && textSize.X > innerDimensions.Width)
            scale *= innerDimensions.Width / textSize.X;

        DynamicSpriteFont font = (isLarge ? FontAssets.DeathText : FontAssets.MouseText).Value;
        Vector2 size = font.MeasureString(visibleText);
        Color baseColor = shadowColor * (color.A / 255f);
        Vector2 origin = new Vector2(0f, 0f) * size;
        Vector2 baseScale = new(scale);
        TextSnippet[] snippets = ChatManager.ParseMessage(visibleText, color).ToArray();

        if(!AllowSnippets)
            ChatManager.ConvertNormalSnippets(snippets);

        ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position, baseColor, 0f, origin, baseScale, -1f, 1.5f);
        ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, 0f, origin, baseScale, out _, -1f, ignoreColors: true);
    }

    private void VerifyTextState()
    {
        if ((object)lastTextReference != Text)
            InternalSetText(text, textScale, isLarge);
    }

    private void InternalSetText(object text, float textScale, bool large)
    {
        DynamicSpriteFont dynamicSpriteFont = (large ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);
        this.text = text;
        isLarge = large;
        this.textScale = textScale;
        lastTextReference = text.ToString();

        if (IsWrapped)
            visibleText = dynamicSpriteFont.CreateWrappedText(lastTextReference, GetInnerDimensions().Width / textScale);
        else
            visibleText = lastTextReference;

        Vector2 size = ChatManager.GetStringSize(dynamicSpriteFont, visibleText, new Vector2(1));
        Vector2 adjustedSize = (textSize = ((!IsWrapped) ? (new Vector2(size.X, large ? 32f : 16f) * textScale) : (new Vector2(size.X, size.Y + WrappedTextBottomPadding) * textScale)));
       
        if (!IsWrapped)
             MinWidth.Set(adjustedSize.X + PaddingLeft + PaddingRight, 0f);

        MinHeight.Set(adjustedSize.Y + PaddingTop + PaddingBottom, 0f);
        OnInternalTextChange?.Invoke();
    }
}
