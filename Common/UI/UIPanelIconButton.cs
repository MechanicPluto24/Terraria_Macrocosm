using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIPanelIconButton : UIHoverImageButton
    {
        public Color IconColor { get; set; } = Color.White;
        public bool GrayscaleIconIfNotInteractible { get; set; } = false;
        public bool OverrideBackgroundColor { get; set; } = false;

        public Color BackPanelColor { get; set; } = UITheme.Current.ButtonStyle.BackgroundColor;
        public Color FocusedBackPanelColor { get; set; } = UITheme.Current.ButtonHighlightStyle.BackgroundColor;
        public Color NotInteractibleBackPanelColor { get; set; } = UITheme.Current.ButtonStyle.BackgroundColor.WithSaturation(0.5f);

        public Color BackPanelBorderColor { get; set; } = UITheme.Current.ButtonStyle.BorderColor;
        public Color BackPanelHoverBorderColor { get; set; } = UITheme.Current.ButtonHighlightStyle.BorderColor;

        private Asset<Texture2D> backPanelTexture;
        private Asset<Texture2D> backPanelBorderTexture;

        public UIPanelIconButton() : this(Macrocosm.EmptyTexAsset) { }

        public UIPanelIconButton(Asset<Texture2D> texture) :
            this(texture,
                 ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/SmallPanel", AssetRequestMode.ImmediateLoad),
                 ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/SmallPanelBorder", AssetRequestMode.ImmediateLoad),
                 ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/SmallPanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
        { }

        public UIPanelIconButton(
            Asset<Texture2D> texture,
            Asset<Texture2D> backPanelTexture,
            Asset<Texture2D> backPanelBorderTexture,
            Asset<Texture2D> backPanelHoverBorderTexture)
            : base(texture, backPanelHoverBorderTexture)
        {
            this.backPanelTexture = backPanelTexture;
            this.backPanelBorderTexture = backPanelBorderTexture;

            Width.Set(backPanelTexture.Width(), 0f);
            Height.Set(backPanelTexture.Height(), 0f);
        }

        public void SetIcon(Asset<Texture2D> texture)
        {
            var width = Width;
            var height = Height;
            SetImage(texture);
            Width = width;
            Height = height;
        }


        SpriteBatchState state;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            bool interactible = CheckInteractible();

            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height) / 2f;

            Color backPanelBorderColor = BackPanelBorderColor * (IsMouseHovering && CheckInteractible() ? visibilityHover : visibilityInteractible);
            Color backPanelColor;

            if (!interactible)
                backPanelColor = BackPanelColor.WithSaturation(0.25f);
            else if ((HasFocus || IsMouseHovering) && !OverrideBackgroundColor)
                backPanelColor = FocusedBackPanelColor;
            else
                backPanelColor = BackPanelColor;

            if (!OverrideBackgroundColor)
                backPanelColor *= IsMouseHovering && interactible ? visibilityHover : visibilityInteractible;

            spriteBatch.Draw(backPanelTexture.Value, position, null, backPanelColor, 0f, backPanelTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(backPanelBorderTexture.Value, position, null, backPanelBorderColor, 0f, backPanelBorderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            if ((IsMouseHovering || (HasFocus && DrawBorderIfInFocus) || remoteInteractionFeedbackTicks > 0) && interactible)
                spriteBatch.Draw(borderTexture.Value, position, null, BackPanelHoverBorderColor, 0f, borderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            if (GrayscaleIconIfNotInteractible && !interactible)
            {
                Effect grayscaleEffect = ModContent.Request<Effect>("Macrocosm/Assets/Effects/Grayscale").Value;
                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(grayscaleEffect, state);
            }

            spriteBatch.Draw(texture.Value, position, null, IconColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            if (GrayscaleIconIfNotInteractible && !interactible)
            {
                spriteBatch.End();
                spriteBatch.Begin(state);
            }
        }
    }
}
