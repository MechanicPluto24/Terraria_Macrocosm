using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIListScrollablePanel : UIPanel
    {
        public bool HideScrollbarIfNotScrollable { get; set; } = true;

        public float ListPadding { get; set; } = 3f;

        private UIText title;
        private UIList list;
        private UIScrollbar scrollbar;

        private bool hasTitle = false;

		public UIListScrollablePanel()
		{
			list = new()
			{
				Width = new(0f, 1f),
				Height = new(0f, 1f)
			};
		}

		public UIListScrollablePanel(LocalizedColorScaleText title)
        {
			this.title = title.ProvideUI();

			list = new()
			{
				Width = new(0f, 1f),
				Height = new(0f, 1f)
			};
		}

        public UIListScrollablePanel(LocalizedText title)
        {
            this.title = new(title);

			list = new()
			{
				Width = new(0f, 1f),
				Height = new(0f, 1f)
			};
		}

		public UIListScrollablePanel(string title)
		{
			this.title = new(title);

			list = new()
			{
				Width = new(0f, 1f),
				Height = new(0f, 1f)
			};
		}

		public UIListScrollablePanel(UIList list, LocalizedColorScaleText title = null)
		{
			if (title is not null)
				this.title = title.ProvideUI();

			this.list = list;
		}

		public override void OnInitialize()
        {
			list ??= new();
			list.Width = new(0f, 1f);
			list.Height = new(0f, 1f);
			list.SetPadding(6f);
            list.ListPadding = ListPadding;

            if (title is not null)
            {
				title.Top = new(15, 0);
                title.HAlign = 0.5f;
          
                Append(title);
                title.Recalculate();
                list.PaddingTop = title.GetDimensions().Height * 2.5f;

                hasTitle = true;
            }

            Append(list);
        }

		public bool EntireListVisible()
		{
			list.Recalculate();
			list.RecalculateChildren();
			float listHeight = list.GetTotalHeight();
			float panelHeight = list.GetInnerDimensions().Height;

			return listHeight < panelHeight;
		}

		public void Add(UIElement element)
		{
			list.Add(element);
		}

		public void AddList(List<UIElement> elements)
		{
			foreach (UIElement element in elements)
				Add(element);
		}

		public void ClearList() => list.Clear();

		public void AppendSeparator()
		{
			Add(new UIHorizontalSeparator()
			{
				Width = StyleDimension.FromPercent(0.98f),
				Color = new Color(89, 116, 213, 255) * 0.9f
			});
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Method is only called if dynamic scrollbar hiding is active
			if (!HideScrollbarIfNotScrollable || !EntireListVisible())
            {
                if (scrollbar is null)
                {
                    scrollbar = new();
                    scrollbar.Height.Set(0f, 0.94f);
                    scrollbar.HAlign = 0.98f;
                    scrollbar.VAlign = 0.5f;
                    list.SetScrollbar(scrollbar);
                    Append(scrollbar);

                    list.Width.Set(0, 0.92f);

                    if(hasTitle)
                        title.Left = new(0, -0.05f);
                }
            }
            else
            {
                if (Children.Contains(scrollbar))
                    RemoveChild(scrollbar);

				list.Width.Set(0, 1f);

				scrollbar = null;
            }
        }
	}
}
