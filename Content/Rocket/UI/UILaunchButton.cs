

using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UILaunchButton : UIElement
	{
		public string Text = "";
		public Color TextColor;
		
		UIText buttonText;
		UIPanel buttonPanel;


		public override void OnInitialize()
		{
			Width.Set(305, 0);
			Height.Set(75, 0);
			HAlign = 0.5f;
			VAlign = 0.945f;

			buttonPanel = new();
			buttonPanel.Width.Set(Width.Pixels, 0);
			buttonPanel.Height.Set(Height.Pixels, 0);
			
			buttonText = new("", 1.1f, true)
			{
				HAlign = 0.5f,
				VAlign = 0.5f,
				TextColor = Color.White
			};

			buttonPanel.Append(buttonText);
			Append(buttonPanel);
		}

		public override void Update(GameTime gameTime)
		{
			buttonText.TextColor = TextColor;
			buttonText.SetText(Text);

			if (IsMouseHovering)
			{
				buttonPanel.BorderColor = Color.Gold;
				buttonPanel.BackgroundColor = (new Color(63, 82, 151) * 0.8f).NewAlpha(0.7f);
			}
			else
			{
				buttonPanel.BorderColor = Color.Black;
				buttonPanel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			}
		}
	}
}
