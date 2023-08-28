using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    // TODO: make an abstract scrollable panel class
    internal class UICrewPanel : UIPanel
    {
        private LocalizedText title;

        private UIText uIDisplayName;
        private UIList uIInfoElements;

        public UICrewPanel(LocalizedText title)
        {
            this.title = title;
            Initialize();
        }

        public override void OnInitialize()
        {
            Width.Set(245, 0);
            Height.Set(400, 0);
            Left.Set(10, 0f);
            Top.Set(249, 0f);
            SetPadding(0f);
            BorderColor = new Color(89, 116, 213, 255);
			BackgroundColor = new Color(53, 72, 135);

			UIList uIList = new()
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f)
            };

            uIList.SetPadding(2f);
            uIList.PaddingBottom = 4f;
            uIList.PaddingTop = 50f;
            uIList.PaddingLeft = 6f;
            Append(uIList);
            uIInfoElements = uIList;

            uIList.ListPadding = 4f;
            uIList.ManualSortMethod = (_) => { };

            UIScrollbar uIScrollbar = new UIScrollbar();
            uIScrollbar.SetView(150f, 1000f);
            uIScrollbar.Height.Set(0f, 0.95f);
            uIScrollbar.HAlign = 0.99f;
            uIScrollbar.VAlign = 0.5f;
            uIInfoElements.SetScrollbar(uIScrollbar);

            Append(uIScrollbar);
            uIInfoElements.Width.Set(-20f, 1f);

            uIDisplayName = new(title, 1.5f, false)
            {
                HAlign = 0.43f,
                Top = new StyleDimension(15, 0f),
                TextColor = Color.White
            };
            Append(uIDisplayName);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            uIDisplayName.SetText(title);
        }

        public void Add(UIElement element)
        {
            uIInfoElements.Add(element);
        }

        public void ClearInfo() => uIInfoElements.Clear();

    }
}
