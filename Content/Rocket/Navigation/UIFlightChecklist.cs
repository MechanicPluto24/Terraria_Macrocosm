using Microsoft.Xna.Framework;
using Terraria.UI;
using Terraria.Localization;
using Terraria.GameContent.UI.Elements;
using System.Collections.Generic;
using Macrocosm.Content.Rocket.Navigation.InfoElements;
using Macrocosm.Content.Rocket.Navigation.LaunchConds;
using System.Xml.Linq;

namespace Macrocosm.Content.Rocket.Navigation
{
    // TODO: make an abstract scrollable list panel class
    public class UIFlightChecklist : UIPanel
    {
        private UIText uIDisplayName;
        private UIList uIInfoElements;

        private string title;
        public UIFlightChecklist(string title)
        {
            this.title = title;
            Initialize();
        }

        public override void OnInitialize()
        {
            Width.Set(245, 0);
            Height.Set(200, 0);
            Left.Set(576, 0f);
            Top.Set(249, 0f);
            SetPadding(0f);
            BorderColor = new Color(89, 116, 213, 255);
            BackgroundColor = new Color(73, 94, 171);

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
            uIScrollbar.Height.Set(0f, 0.89f);
            uIScrollbar.HAlign = 0.99f;
            uIScrollbar.VAlign = 0.5f;
            uIInfoElements.SetScrollbar(uIScrollbar);

            Append(uIScrollbar);
            uIInfoElements.Width.Set(-20f, 1f);

            uIDisplayName = new(title, 1.2f, false)
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

            ClearInfo();
        }

        public void Add(UIElement element)
        {
            uIInfoElements.Add(element);
        }

		public void AddList(List<UIElement> elements)
		{
            foreach(UIElement element in elements)
			    uIInfoElements.Add(element);
		}


		public void ClearInfo() => uIInfoElements.Clear();

    }
}
