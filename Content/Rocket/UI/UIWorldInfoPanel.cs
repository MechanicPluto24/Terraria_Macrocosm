

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
			Width.Set(245, 0);
			Height.Set(400, 0);
			Left.Set(10, 0f);
			Top.Set(249, 0f);
			Recalculate();

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
				TextColor = Color.White
			};
			UIDisplayText.Left.Set(6f, 0f);
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
