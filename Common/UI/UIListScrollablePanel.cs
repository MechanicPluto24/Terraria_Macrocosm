using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIListScrollablePanel : UIPanel, IEnumerable<UIElement>
    {
        public bool HideScrollbarIfNotScrollable { get; set; } = true;
        public float ListPadding { get; set; } = 3f;
        public float ListOuterPadding { get; set; } = 6f;
        public StyleDimension ListWidthWithScrollbar { get; set; } = new(0f, 0.915f);
        public StyleDimension ListWidthWithoutScrollbar { get; set; } = new(0f, 1f);
        public StyleDimension ScrollbarWidth { get; set; } = new(0f, 1f);
        public StyleDimension ScrollbarHeight { get; set; } = new(0f, 0.94f);
        public StyleDimension ScrollbarTop { get; set; } = default;
        public StyleDimension ScrollbarLeft { get; set; } = default;

        public float ScrollbarHAlign { get; set; } = 0.98f;
        public float ScrollbarVAlign { get; set; } = 0.5f;
        public float TitleHAlign { get; set; } = 0.5f;

        public bool HasScrollbar => scrollbar is not null;
        public bool ShiftTitleIfHasScrollbar { get; set; } = true;
        public bool HasTitle => title is not null;

        private UIText title;
        private UIList list;
        private UIScrollbar scrollbar;
        private bool hasTitle = false;

        public UIListScrollablePanel()
        {
            list = new();
        }

        public UIListScrollablePanel(string titleKey) : this(Language.GetText(titleKey))
        {
        }

        public UIListScrollablePanel(LocalizedText title) : this(new LocalizedColorScaleText(title))
        {
        }

        public UIListScrollablePanel(LocalizedColorScaleText title) : this()
        {
            this.title = title.ProvideUIText();
        }

        public UIListScrollablePanel(List<UIElement> list, LocalizedColorScaleText title = null)
        {
            this.title = title?.ProvideUIText();

            this.list = new();
            list.AddRange(list);
        }

        public UIListScrollablePanel(UIList list, LocalizedColorScaleText title = null)
        {
            this.title = title?.ProvideUIText();
            this.list = list;
        }

        public override void OnInitialize()
        {
            list ??= new();
            list.Width = ListWidthWithoutScrollbar;
            list.Height = new(0f, 1f);
            list.SetPadding(ListOuterPadding);
            list.ListPadding = ListPadding;

            if (title is not null)
            {
                title.Top = new(15, 0);
                title.HAlign = TitleHAlign;

                Append(title);
                title.Recalculate();
                list.PaddingTop = title.GetDimensions().Height * 2.5f;
                hasTitle = true;
            }

            Append(list);

            CheckScrollbar();
        }

        public void UpdateOrder() => list.UpdateOrder();
        public Action<List<UIElement>> ManualSortMethod
        {
            get => list.ManualSortMethod;
            set => list.ManualSortMethod = value;
        }

        public void SetTitle(string titleKey) => SetTitle(Language.GetText(titleKey));
        public void SetTitle(LocalizedText title) => SetTitle(new LocalizedColorScaleText(title));

        public void SetTitle(LocalizedColorScaleText title)
        {
            if (this.title is not null)
                RemoveChild(this.title);

            this.title = title.ProvideUIText();
            Initialize();
        }

        public bool EntireListVisible()
        {
            list.Recalculate();
            list.RecalculateChildren();
            float listHeight = list.GetTotalHeight();
            float panelHeight = list.GetInnerDimensions().Height;

            return listHeight < panelHeight || panelHeight == 0;
        }

        public void Add(UIElement element) => list.Add(element);
        public void AddRange(List<UIElement> elements) => list.AddRange(elements);
        public void RemoveFromList(UIElement element) => list.Remove(element);
        public void ClearList() => list.Clear();

        public IEnumerable<T> OfType<T>(bool recursive = true) where T : UIElement
        {
            if (!recursive)
                return list.OfType<T>();

            List<T> result = new();
            foreach (UIElement element in list)
            {
                element.ExecuteRecursively(child =>
                {
                    if (child is T typedChild)
                        result.Add(typedChild);
                });
            }

            return result;
        }

        public UIHorizontalSeparator AddHorizontalSeparator(float percent = 0.98f, float hAlign = 0.5f)
        {
            var separator = new UIHorizontalSeparator()
            {
                Width = StyleDimension.FromPercent(percent),
                HAlign = hAlign,
                Color = UITheme.Current.SeparatorColor
            };

            Add(separator);
            return separator;
        }

        public UIHorizontalSeparator InsertHorizontalSeparator(int index)
        {
            var separator = new UIHorizontalSeparator()
            {
                Width = StyleDimension.FromPercent(0.98f),
                Color = UITheme.Current.SeparatorColor
            };

            // Insert the separator at the specified index
            list.ToList().Insert(index, separator);

            return separator;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CheckScrollbar();

            if (hasTitle && title.HAlign != TitleHAlign)
                title.HAlign = TitleHAlign;
        }

        private void CheckScrollbar()
        {
            if (!HideScrollbarIfNotScrollable || !EntireListVisible())
            {
                if (scrollbar is null)
                {
                    scrollbar = new()
                    {
                        HAlign = ScrollbarHAlign,
                        VAlign = ScrollbarVAlign,
                        Height = ScrollbarHeight,
                        Width = ScrollbarWidth,
                        Top = ScrollbarTop,
                        Left = ScrollbarLeft
                    };
                    list.SetScrollbar(scrollbar);
                    Append(scrollbar);

                    list.Width = ListWidthWithScrollbar;

                    if (hasTitle && ShiftTitleIfHasScrollbar)
                        title.Left = new(0, -0.05f);
                }
            }
            else
            {
                if (Children.Contains(scrollbar))
                    RemoveChild(scrollbar);

                list.Width = ListWidthWithoutScrollbar;

                scrollbar = null;
            }
        }

        public IEnumerator<UIElement> GetEnumerator()
        {
            return ((IEnumerable<UIElement>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}
