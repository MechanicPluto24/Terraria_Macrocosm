using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.UI;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.UI.Themes;

namespace Macrocosm.Common.UI
{
    public class UICustomScrollbar : UIElement
    {
        private float viewPosition;
        private float viewSize = 1f;
        private float maxViewSize = 20f;
        private bool isDragging;
        private bool isHoveringOverHandle;
        private float dragYOffset;

        private static Asset<Texture2D> fill1Texture;
        private static Asset<Texture2D> fill2Texture;
        private static Asset<Texture2D> borderTexture;
        private static Asset<Texture2D> innerTexture;
        private static Asset<Texture2D> innerBorderTexture;

        public Color Fill1Color { get; set; }
        public Color Fill2Color { get; set; }
        public Color BorderColor { get; set; }
        public Color InnerColor { get; set; }
        public Color InnerBorderColor { get; set; }

        public float ViewPosition
        {
            get => viewPosition;
            set => viewPosition = MathHelper.Clamp(value, 0f, maxViewSize - viewSize);
        }
        public float ViewSize => viewSize;
        public float MaxViewSize => maxViewSize;

        public bool CanScroll => maxViewSize != viewSize;
        public void GoToBottom() => ViewPosition = maxViewSize - viewSize;

        public UICustomScrollbar()
        {
            Width.Set(20f, 0f);
            MaxWidth.Set(20f, 0f);
            PaddingTop = 5f;
            PaddingBottom = 5f;

            fill1Texture ??= ModContent.Request<Texture2D>(Macrocosm.UITexturesPath + "ScrollbarFill1");
            fill2Texture ??= ModContent.Request<Texture2D>(Macrocosm.UITexturesPath + "ScrollbarFill2");
            borderTexture ??= ModContent.Request<Texture2D>(Macrocosm.UITexturesPath + "ScrollbarBorder");
            innerTexture ??= ModContent.Request<Texture2D>(Macrocosm.UITexturesPath + "ScrollbarInner");
            innerBorderTexture ??= ModContent.Request<Texture2D>(Macrocosm.UITexturesPath + "ScrollbarInnerBorder");

            var style = UITheme.Current.ScrollbarStyle;
            Fill1Color = style.Fill1Color;
            Fill2Color = style.Fill2Color;
            BorderColor = style.BorderColor;
            InnerColor = style.InnerColor;
            InnerBorderColor = style.InnerBorderColor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
                PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            viewPosition = MathHelper.Clamp(viewPosition, 0f, maxViewSize - viewSize);
            this.viewSize = viewSize;
            this.maxViewSize = maxViewSize;
        }

        public float GetValue() => viewPosition;

        private Rectangle GetHandleRectangle()
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (maxViewSize == 0f && viewSize == 0f)
            {
                viewSize = 1f;
                maxViewSize = 1f;
            }

            return new Rectangle((int)innerDimensions.X, (int)(innerDimensions.Y + innerDimensions.Height * (viewPosition / maxViewSize)) - 3, 20, (int)(innerDimensions.Height * (viewSize / maxViewSize)) + 7);
        }

        internal void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
        {
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y - 6, dimensions.Width, 6), new Rectangle(0, 0, texture.Width, 6), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle(0, 6, texture.Width, 4), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + dimensions.Height, dimensions.Width, 6), new Rectangle(0, texture.Height - 6, texture.Width, 6), color);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (isDragging)
            {
                float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - dragYOffset;
                viewPosition = MathHelper.Clamp(num / innerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
            }

            Rectangle handleRectangle = GetHandleRectangle();
            Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;
            bool isHoveringOverHandle = this.isHoveringOverHandle;
            this.isHoveringOverHandle = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));
            if (!isHoveringOverHandle && this.isHoveringOverHandle && Main.hasFocus)
                SoundEngine.PlaySound(SoundID.MenuTick);

            DrawBar(spriteBatch, fill1Texture.Value, dimensions.ToRectangle(), Fill1Color);
            DrawBar(spriteBatch, fill2Texture.Value, dimensions.ToRectangle(), Fill2Color);
            DrawBar(spriteBatch, borderTexture.Value, dimensions.ToRectangle(), BorderColor);
            DrawBar(spriteBatch, innerTexture.Value, handleRectangle, InnerColor * ((isDragging || this.isHoveringOverHandle) ? 1f : 0.85f));
            DrawBar(spriteBatch, innerBorderTexture.Value, handleRectangle, InnerBorderColor * ((isDragging || this.isHoveringOverHandle) ? 1f : 0.85f));
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            if (evt.Target == this)
            {
                Rectangle handleRectangle = GetHandleRectangle();
                if (handleRectangle.Contains(new Point((int)evt.MousePosition.X, (int)evt.MousePosition.Y)))
                {
                    isDragging = true;
                    dragYOffset = evt.MousePosition.Y - (float)handleRectangle.Y;
                }
                else
                {
                    CalculatedStyle innerDimensions = GetInnerDimensions();
                    float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - (float)(handleRectangle.Height >> 1);
                    viewPosition = MathHelper.Clamp(num / innerDimensions.Height * maxViewSize, 0f, maxViewSize - viewSize);
                }
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            isDragging = false;
        }
    }
}
