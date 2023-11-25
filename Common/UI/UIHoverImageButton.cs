using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIHoverImageButton : UIElement, IFocusable
    {
        /// <summary> Tooltip text, shown on hover </summary>
        public LocalizedText HoverText { get; set; }

        /// <summary> Function to determine whether this button can be interacted with </summary>
        public Func<bool> CheckInteractible { get; set; } = () => true;

        /// <summary> Whether to display hover text even if the button is not interactible </summary>
        public bool HoverTextOnButonNotInteractible { get; set; } = false;

        public bool DrawBorderIfInFocus { get; set; } = true;

        public bool HasFocus { get; set; }
        public string FocusContext { get; set; }

        public Action OnFocusGain { get; set; } = () => { };
        public Action OnFocusLost { get; set; } = () => { };


        protected Asset<Texture2D> texture;

        protected Asset<Texture2D> borderTexture;


        protected int remoteInteractionFeedbackTicks = 0;

        protected float visibilityInteractible = 1f;

        protected float visibilityHover = 0.8f;

        protected float visibilityNotInteractible = 0.4f;


        public UIHoverImageButton(Asset<Texture2D> texture, Asset<Texture2D> borderTexture = null, LocalizedText hoverText = null)
        {
            if (hoverText is not null)
                HoverText = hoverText;

            this.texture = texture;
            this.borderTexture = borderTexture;
            Width.Set(texture.Width(), 0f);
            Height.Set(texture.Height(), 0f);
        }

        public void SetImage(Asset<Texture2D> texture)
        {
            this.texture = texture;
            Width.Set(this.texture.Width(), 0f);
            Height.Set(this.texture.Height(), 0f);
        }

        public void SetBorderTexture(Asset<Texture2D> borderTexture)
        {
            this.borderTexture = borderTexture;
        }

        public void SetVisibility(float whenInteractible, float whenNotInteractible, float whenHovering)
        {
            visibilityInteractible = MathHelper.Clamp(whenInteractible, 0f, 1f);
            visibilityNotInteractible = MathHelper.Clamp(whenNotInteractible, 0f, 1f);
            visibilityHover = MathHelper.Clamp(whenHovering, 0f, 1f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering && HoverText is not null && (HoverTextOnButonNotInteractible || CheckInteractible()))
                Main.instance.MouseText(HoverText.Value, "", 0, 0, hackedMouseX: Main.mouseX + 6, hackedMouseY: Main.mouseY + 6, noOverride: true);

            if (remoteInteractionFeedbackTicks > 0)
                remoteInteractionFeedbackTicks--;
        }

        public void TriggerRemoteInteraction(int ticks = 10)
        {
            if (!CheckInteractible())
                return;

            remoteInteractionFeedbackTicks = ticks;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            float visibility = CheckInteractible() ? (IsMouseHovering ? visibilityHover : visibilityInteractible) : visibilityNotInteractible;
            spriteBatch.Draw(texture.Value, dimensions.Position(), Color.White * visibility);

            if (borderTexture != null && (IsMouseHovering && CheckInteractible()) || remoteInteractionFeedbackTicks > 0 || HasFocus && DrawBorderIfInFocus)
                spriteBatch.Draw(borderTexture.Value, dimensions.Position(), Color.White);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (CheckInteractible())
                base.LeftClick(evt);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            if (CheckInteractible())
                SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
}
