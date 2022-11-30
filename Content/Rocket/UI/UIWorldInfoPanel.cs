

using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UIWorldInfoPanel : UIElement
	{
		public string Name = "";
		public string Text = "";

		private UIText UIDisplayName;
		private UIText UIDisplayText;
		
		public override void OnInitialize()
		{
			Width.Set(250, 0);
			Height.Set(400, 0);
			HAlign = 0.05f;
			VAlign = 0.9f;

			UIPanel panel = new();
			panel.Width.Set(Width.Pixels, 0);
			panel.Height.Set(Height.Pixels, 0);

			UIDisplayName = new(Text, 1.5f, false)
			{
				HAlign = 0.5f,
				VAlign = 0f,
				TextColor = Color.White
			};

			UIDisplayText = new(Text, 1.1f, false)
			{
				HAlign = 0.2f,
				TextColor = Color.White
			};
			UIDisplayText.Top.Set(40, 0f);

			panel.Append(UIDisplayText);
			panel.Append(UIDisplayName);
			Append(panel);
		}

		public override void Update(GameTime gameTime)
		{
			UIDisplayText.SetText(Text);
			UIDisplayName.SetText(Name);
		}
	}
}
