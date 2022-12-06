

using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UIWorldInfoElement : UIPanel
	{
		public Texture2D Icon;
		public string DisplayValue = "";
		public string HoverText = "";

		private UIPanel Background;
		private UIText DisplayText;

		public UIWorldInfoElement(Texture2D icon, string value, string hoverText)
		{
			Icon = icon;
			DisplayValue = value;
			HoverText = hoverText;

			Width.Set(0f, 0.98f);
			Height.Set(38f, 0f);

			BackgroundColor = new Color(43, 56, 101);
			BorderColor = BackgroundColor * 1.1f;

			DisplayText = new(DisplayValue, 0.9f, false);
			DisplayText.VAlign = 0.5f;
			DisplayText.Left.Set(40, 0f);

			Append(DisplayText);
		}
		
		public override void OnInitialize()
		{
			
		}

		private void Background_OnUpdate(UIElement affectedElement)
		{

		}

		public override void Update(GameTime gameTime)
		{

			if (IsMouseHovering)
				Main.instance.MouseText(HoverText);

			if (DisplayText is not null && !DisplayText.Text.Equals(DisplayValue))
				DisplayText.SetText(DisplayValue);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle dimensions = GetDimensions();

			

			spriteBatch.Draw(Icon, dimensions.Position() + new Vector2(dimensions.Width * 0.062f, dimensions.Height * 0.18f), Color.White);
		}
	}
}
