using Macrocosm.Common.UI.Themes;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIDynamicTextPanel : UIPanel
    {
        private UIText uIText;

        public UIDynamicTextPanel(LocalizedColorScaleText text)
        {
            uIText = text.ProvideUI();
        }

        public UIDynamicTextPanel(LocalizedText text, float textScale = 1f, bool large = false)
        {
            uIText = new(text, textScale, large);
        }

        public UIDynamicTextPanel(string text, float textScale = 1f, bool large = false)
        {
            uIText = new(text, textScale, large);
        }

        public void SetText(string text) => uIText.SetText(text);
        public void SetText(LocalizedText text) => uIText.SetText(text);

        public override void OnInitialize()
        {
            Width.Set(0f, 1f);
            Height.Set(1f, 0f);
            BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor;
            BorderColor = UITheme.Current.InfoElementStyle.BorderColor;
            PaddingLeft = 4f;
            PaddingRight = 4f;

            uIText.HAlign = 0f;
            uIText.VAlign = 0f;
            uIText.Width = StyleDimension.FromPixelsAndPercent(0f, 1f);
            uIText.Height = StyleDimension.FromPixelsAndPercent(0f, 1f);
            uIText.PaddingLeft = 8f;
            uIText.PaddingRight = 8f;
            uIText.IsWrapped = true;

            AddDynamicResize(this, uIText);
            Append(uIText);
        }

        private void AddDynamicResize(UIElement container, UIText text)
        {
            text.OnInternalTextChange += delegate
            {
                container.Height = new StyleDimension(text.MinHeight.Pixels, 0f);
            };
        }
    }
}
