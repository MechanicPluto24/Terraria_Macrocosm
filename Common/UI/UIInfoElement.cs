using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIInfoElement : UIPanel
    {
        protected readonly Asset<Texture2D> icon;
        protected readonly Asset<Texture2D> iconSymbol;

        protected readonly LocalizedColorScaleText displayText;
        protected readonly LocalizedText hoverText;

        protected UIText uIDisplayText;

        public float IconHAlign { get; set; } = 0.1f;

        public UIInfoElement(LocalizedColorScaleText displayText, Asset<Texture2D> icon = null, Asset<Texture2D> iconSymbol = null, LocalizedText hoverText = null)
        {
            this.icon = icon ?? Macrocosm.EmptyTexAsset;
            this.iconSymbol = iconSymbol ?? Macrocosm.EmptyTexAsset;
            this.displayText = displayText;

            if (hoverText is null)
                this.hoverText = LocalizedText.Empty;
            else
                this.hoverText = hoverText;
        }

        public UIInfoElement(LocalizedText displayText, Asset<Texture2D> icon = null, Asset<Texture2D> iconSymbol = null, LocalizedText hoverText = null) : this(new LocalizedColorScaleText(displayText), icon, iconSymbol, hoverText)
        {
        }

        public UIInfoElement(string displayText, Asset<Texture2D> icon = null, Asset<Texture2D> iconSymbol = null, LocalizedText hoverText = null) : this(Language.GetText(displayText), icon, iconSymbol, hoverText)
        {
        }

        public override void OnInitialize()
        {
            Width.Set(0f, 1f);
            Height.Set(40f, 0f);

            BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor;
            BorderColor = UITheme.Current.InfoElementStyle.BorderColor;

            uIDisplayText = displayText.ProvideUI();
            uIDisplayText.Left = new StyleDimension(40, 0);
            uIDisplayText.VAlign = 0.5f;

            Append(uIDisplayText);
        }

        public void SetTextLeft(float pixels, float percent)
        {
            uIDisplayText.Left = new StyleDimension(pixels, percent);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
                Main.instance.MouseText(hoverText.Value);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Recalculate();
            CalculatedStyle dimensions = GetDimensions();
            Vector2 iconPosition = dimensions.Position() + new Vector2(dimensions.Width * IconHAlign, dimensions.Height / 2f);
            spriteBatch.Draw(icon.Value, iconPosition, null, Color.White, 0f, new Vector2(icon.Width() * 0.5f, icon.Height() * 0.5f), 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(iconSymbol.Value, iconPosition + new Vector2(6f, 0f), null, Color.White, 0f, new Vector2(icon.Width() * 0.5f, icon.Height() * 0.5f), 1f, SpriteEffects.None, 0);
        }
    }
}
