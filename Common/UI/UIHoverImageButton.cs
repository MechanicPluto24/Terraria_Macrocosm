using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
	public class UIHoverImageButton : UIElement
	{
		/// <summary> Tooltip text, shown on hover </summary>
		public LocalizedText HoverText { get; set; }

		/// <summary> Function to determine whether this button can be interacted with </summary>
		public Func<bool> CheckInteractible { get; set; } = () => true;

		/// <summary> Whether to display hover text even if the button is not interactible </summary>
		public bool HoverTextOnButonNotInteractible { get; set; } = false;


		protected Asset<Texture2D> texture;

		protected Asset<Texture2D> borderTexture;


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

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();

			float visibility = CheckInteractible() ? (IsMouseHovering ? visibilityHover : visibilityInteractible ) : visibilityNotInteractible;
			spriteBatch.Draw(texture.Value, dimensions.Position(), Color.White * visibility);
			
			if (borderTexture != null && IsMouseHovering && CheckInteractible())
 				spriteBatch.Draw(borderTexture.Value, dimensions.Position(), Color.White);
 
			if (IsMouseHovering && HoverText is not null && (HoverTextOnButonNotInteractible || CheckInteractible()))
				Main.hoverItemName = HoverText.Value;
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);

			if(CheckInteractible())
				SoundEngine.PlaySound(SoundID.MenuTick);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
		}
	}
}
