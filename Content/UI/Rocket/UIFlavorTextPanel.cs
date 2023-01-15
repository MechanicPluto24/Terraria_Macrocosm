using Macrocosm.Common.Utils;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
{
    public class UIFlavorTextPanel : UIPanel
    {
        private string text;

        public UIFlavorTextPanel(string text)
        {
            this.text = text;
            Initialize();
        }

        public override void OnInitialize()
        {
            Width.Set(0f, 0.98f);
            Height.Set(100f, 0f);
            BackgroundColor = new Color(43, 56, 101);
            BorderColor = BackgroundColor * 2f;
            PaddingLeft = 4f;
            PaddingRight = 4f;

            UIText uIText = new(text, 0.8f)
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                PaddingLeft = 8f,
                PaddingRight = 8f,
                IsWrapped = true
            };
            AddDynamicResize(this, uIText);
            Append(uIText);
        }

        private static void AddDynamicResize(UIElement container, UIText text)
        {
            text.OnInternalTextChange += delegate
            {
                container.Height = new StyleDimension(text.MinHeight.Pixels, 0f);
            };
        }
    }
}
