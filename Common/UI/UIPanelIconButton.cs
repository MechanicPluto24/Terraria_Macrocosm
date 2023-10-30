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
		public Color BackPanelHoverBorderColor { get; set; } = Color.Gold;
		public Color FocusedBackPanelColor { get; set; } = new Color(37, 52, 96);

		private Asset<Texture2D> backPanelTexture;
		public Color BackPanelColor { get; set; } = new(53, 72, 135);

		private Asset<Texture2D> backPanelBorderTexture;
		public Color BackPanelBorderColor { get; set; } = new(89, 116, 213);

		public UIPanelIconButton() : this(Macrocosm.EmptyTexAsset) { }

		public UIPanelIconButton(Asset<Texture2D> texture) : 
			this(texture,
				 ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/SmallPanel", AssetRequestMode.ImmediateLoad),
				 ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/SmallPanelBorder", AssetRequestMode.ImmediateLoad),
				 ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/SmallPanelHoverBorder", AssetRequestMode.ImmediateLoad)
			) { }

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

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			Vector2 position = dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height) / 2f;
			spriteBatch.Draw(backPanelTexture.Value, position, null, BackPanelColor * (IsMouseHovering ? visibilityHover : visibilityInteractible), 0f, backPanelTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(backPanelBorderTexture.Value, position, null, BackPanelBorderColor * (IsMouseHovering ? visibilityHover : visibilityInteractible), 0f, backPanelBorderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

			if (IsMouseHovering || HasFocus && DrawBorderIfInFocus || remoteInteractionFeedbackTicks > 0)
				spriteBatch.Draw(borderTexture.Value, position, null, BackPanelHoverBorderColor, 0f, borderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

			spriteBatch.Draw(texture.Value, position, null, IconColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

			if (HasFocus)
 				BackPanelColor = FocusedBackPanelColor;
 			else
 				BackPanelColor = new Color(53, 72, 135);
 		}
	}
}
