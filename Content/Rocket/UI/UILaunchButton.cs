

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
		
		public override void OnInitialize()
		{
			Width.Set(305, 0);
			Height.Set(75, 0);
			HAlign = 0.5f;
			VAlign = 0.945f;

			UIPanel button = new();
			button.Width.Set(Width.Pixels, 0);
			button.Height.Set(Height.Pixels, 0);
			
			buttonText = new("", 1.1f, true)
			{
				HAlign = 0.5f,
				VAlign = 0.5f,
				TextColor = Color.White
			};

			button.Append(buttonText);
			Append(button);
		}

		public override void Update(GameTime gameTime)
		{
			buttonText.TextColor = TextColor;
			buttonText.SetText(Text);
		}
	}
}
