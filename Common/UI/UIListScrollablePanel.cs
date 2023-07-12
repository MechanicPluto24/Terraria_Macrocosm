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

        public float ContainerSize { get; set; } = 0f;

        private UIText title;
        private UIList list;
        private UIScrollbar scrollbar;

        private bool hasTitle = false;

		public UIListScrollablePanel(UIList list, LocalizedColorScaleText title = null)
		{
			if(title is not null)
				this.title = title.ProvideUI();

			this.list = list;
			Initialize();
		}

		public UIListScrollablePanel(LocalizedColorScaleText title = null)
        {
            if(title is not null)
			    this.title = title.ProvideUI();

			list = new()
            {
                Width = new StyleDimension(0f, 1f),
                Height = new StyleDimension(0f, 1f)
            };

            Initialize();
        }

        public UIListScrollablePanel(LocalizedText title = null)
        {
            if (title is not null)
                this.title = new(title);

			list = new()
			{
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(0f, 1f)
			};

			Initialize();
		}

		public UIListScrollablePanel(string title = null)
		{
			if (title is not null)
				this.title = new(title);

			list = new()
			{
				Width = new StyleDimension(0f, 1f),
				Height = new StyleDimension(0f, 1f)
			};

			Initialize();
		}

		public override void OnInitialize()
        {
            list.SetPadding(6f);
            list.Width.Set(0f, 1f);
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

    }
}
