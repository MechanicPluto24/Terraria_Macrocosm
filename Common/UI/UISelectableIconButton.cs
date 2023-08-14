using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.UI;


namespace Macrocosm.Common.UI
{
	public class UISelectableIconButton : UIPanelIconButton
	{
		public Color SelectedBackPanelColor { get; set; } = new Color(37, 52, 96);

		public bool DrawBorderIfSelected { get; set; } = true;

		public Action OnSelected { get; set; } = () => { };
		public Action OnDeselected { get; set; } = () => { };

		private bool selected;
		public bool Selected 
		{
			get => selected;
			set
			{
				if(!selected && value)
					OnSelected();
				
				if(selected && !value)
					OnDeselected();

				selected = value;
			}
		}

		public UISelectableIconButton() : base(Macrocosm.EmptyTexAsset)
		{
		}

		public UISelectableIconButton(Asset<Texture2D> texture) : base(texture)
		{
		}

		public UISelectableIconButton(
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

			if (selected)
			{
				if(DrawBorderIfSelected)
					spriteBatch.Draw(borderTexture.Value, GetDimensions().Position() + new Vector2(GetDimensions().Width, GetDimensions().Height) / 2f, null, BackPanelHoverBorderColor, 0f, borderTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
				
				BackPanelColor = SelectedBackPanelColor;
			}
			else
			{
 				BackPanelColor = new Color(53, 72, 135);
			}
 
		}
	}
}
