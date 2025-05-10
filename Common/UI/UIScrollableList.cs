using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIScrollableList : UIElement, IEnumerable<UIElement>, IEnumerable
    {
        public delegate bool ElementSearchMethod(UIElement element);

        private class UIInnerList : UIElement
        {
            public override bool ContainsPoint(Vector2 point) => true;

            protected override void DrawChildren(SpriteBatch spriteBatch)
            {
                Vector2 position = Parent.GetDimensions().Position();
                Vector2 dimensions = new(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
                foreach (UIElement element in Elements)
                {
                    Vector2 position2 = element.GetDimensions().Position();
                    Vector2 dimensions2 = new(element.GetDimensions().Width, element.GetDimensions().Height);
                    if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
                        element.Draw(spriteBatch);
                }
            }

            public override Rectangle GetViewCullingArea() => Parent.GetDimensions().ToRectangle();
        }

        protected UICustomScrollbar scrollbar;
        internal UIElement innerList = new UIInnerList();
        private float innerListHeight;
        public float ListPadding { get; set; } = 5f;
        public int Count => Items.Count;
        public List<UIElement> Items { get; set; } = new();
        public Action<List<UIElement>> ManualSortMethod { get; set; }

        public UIScrollableList()
        {
            innerList.OverflowHidden = false;
            innerList.Width.Set(0f, 1f);
            innerList.Height.Set(0f, 1f);
            OverflowHidden = true;
            Append(innerList);
        }

        public float ViewPosition
        {
            get => scrollbar.ViewPosition;
            set => scrollbar.ViewPosition = value;
        }

        public virtual void AddRange(IEnumerable<UIElement> items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
                innerList.Append(item);
            }

            UpdateOrder();
            innerList.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
        }

        public float GetTotalHeight() => innerListHeight;

        public void Goto(ElementSearchMethod searchMethod)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (searchMethod(Items[i]))
                {
                    scrollbar.ViewPosition = Items[i].Top.Pixels;
                    break;
                }
            }
        }

        public void Goto(ElementSearchMethod searchMethod, bool center = false)
        {
            var innerDimensionHeight = GetInnerDimensions().Height;
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (searchMethod(item))
                {
                    scrollbar.ViewPosition = item.Top.Pixels;
                    if (center)
                    {
                        scrollbar.ViewPosition = item.Top.Pixels - innerDimensionHeight / 2 + item.GetOuterDimensions().Height / 2;
                    }
                    return;
                }
            }
        }

        public virtual void Add(UIElement item)
        {
            Items.Add(item);
            innerList.Append(item);
            UpdateOrder();
            innerList.Recalculate();
        }

        public virtual bool Remove(UIElement item)
        {
            innerList.RemoveChild(item);
            // If order is stable doesn't make sense to reorder, left because it's in vanilla
            UpdateOrder();
            return Items.Remove(item);
        }

        public virtual void Clear()
        {
            innerList.RemoveAllChildren();
            Items.Clear();
        }

        public override void Recalculate()
        {
            base.Recalculate();
            UpdateScrollbar();
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (scrollbar != null)
                scrollbar.ViewPosition -= evt.ScrollWheelValue;
        }

        public override void RecalculateChildren()
        {
            base.RecalculateChildren();
            float num = 0f;
            for (int i = 0; i < Items.Count; i++)
            {
                float num2 = Items.Count == 1 ? 0f : ListPadding;
                Items[i].Top.Set(num, 0f);
                Items[i].Recalculate();
                num += Items[i].GetOuterDimensions().Height + num2;
            }

            innerListHeight = num;
        }

        private void UpdateScrollbar()
        {
            if (scrollbar != null)
            {
                float height = GetInnerDimensions().Height;
                scrollbar.SetView(height, innerListHeight);
            }
        }

        public void SetScrollbar(UICustomScrollbar scrollbar)
        {
            this.scrollbar = scrollbar;
            UpdateScrollbar();
        }

        public void UpdateOrder()
        {
            if (ManualSortMethod != null)
                ManualSortMethod(Items);
            else
                Items.Sort(SortMethod);

            UpdateScrollbar();
        }

        public int SortMethod(UIElement item1, UIElement item2) => item1.CompareTo(item2);

        public override List<SnapPoint> GetSnapPoints()
        {
            List<SnapPoint> list = new();
            if (GetSnapPoint(out var point))
                list.Add(point);

            foreach (UIElement item in Items)
            {
                list.AddRange(item.GetSnapPoints());
            }

            return list;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (scrollbar != null)
                innerList.Top.Set(0f - scrollbar.GetValue(), 0f);

            Recalculate();
        }

        public IEnumerator<UIElement> GetEnumerator() => ((IEnumerable<UIElement>)Items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<UIElement>)Items).GetEnumerator();
    }
}
