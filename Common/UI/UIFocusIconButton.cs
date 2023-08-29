using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;


namespace Macrocosm.Common.UI
{
	public class UIFocusIconButton : UIPanelIconButton, IFocusable
	{
		public Color FocusedBackPanelColor { get; set; } = new Color(37, 52, 96);

		public bool DrawBorderIfInFocus { get; set; } = true;

		public bool HasFocus { get; set; }
		public string FocusContext { get; set; }

		public Action OnFocusGain { get; set; } = () => { };
		public Action OnFocusLost { get; set; } = () => { };

		public UIFocusIconButton() : base(Macrocosm.EmptyTexAsset)
		{
		}

		public UIFocusIconButton(Asset<Texture2D> texture) : base(texture)
		{
		}

		public UIFocusIconButton(
			Asset<Texture2D> texture,
			Asset<Texture2D> backPanelTexture,
			Asset<Texture2D> backPanelBorderTexture,
			Asset<Texture2D> backPanelHoverBorderTexture)
			: base(texture, backPanelTexture, backPanelBorderTexture, backPanelHoverBorderTexture)
		{
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (HasFocus)
			{
				if(DrawBorderIfInFocus)
					spriteBatch.Draw(borderTexture.Value, GetDimensions().Position() + new Vector2(GetDimensions().Width, GetDimensions().Height) / 2f, null, BackPanelHoverBorderColor, 0f, borderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
				
				BackPanelColor = FocusedBackPanelColor;
			}
			else
			{
 				BackPanelColor = new Color(53, 72, 135);
			}
		}
	}
}
