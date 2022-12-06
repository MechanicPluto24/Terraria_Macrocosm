

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

		public delegate void OnClick_ZoomIn(bool useDefault);
		public OnClick_ZoomIn ZoomIn = (bool useDefault) => { };

		public delegate void OnClick_Embark();
		public OnClick_Embark Embark = () => { };

		public delegate void OnClick_Launch();
		public OnClick_Launch Launch = () => { };

		public bool CanClick;
			
		private UIText buttonText;
		private UIPanel buttonPanel;

		public ButtonStateType ButtonState;
		public enum ButtonStateType
		{
			NoTarget,
			CantReach,
			ZoomIn,
			Embark,
			Launch
		}

		public override void OnInitialize()
		{
			Width.Set(285, 0);
			Height.Set(75, 0);
			HAlign = 0.5f;
			Top.Set(572, 0f);
			Recalculate();

			buttonPanel = new();
			buttonPanel.Width.Set(Width.Pixels, 0);
			buttonPanel.Height.Set(Height.Pixels, 0);

			buttonText = new(Terraria.Localization.LocalizedText.Empty, 0.9f, true)
			{
				IsWrapped = false,
				HAlign = 0.5f,
				VAlign = 0.5f,
				TextColor = Color.White
			};

			buttonPanel.Append(buttonText);
			Append(buttonPanel);
		}

		public override void Update(GameTime gameTime)
		{

			OnClick -= UILaunchButton_OnClick_Embark;
			OnClick -= UILaunchButton_OnClick_Launch;
			OnClick -= UILaunchButton_OnClick_ZoomIn;
			CanClick = true;

			switch (ButtonState)
			{
				case ButtonStateType.NoTarget:
					TextColor = Color.Gold;
					Text = "NO TARGET";
					CanClick = false;
					break;

				case ButtonStateType.CantReach:
					TextColor = Color.Red;
					Text = "CAN'T REACH";
					CanClick = false;
					break;

				case ButtonStateType.ZoomIn:
					TextColor = Color.White;
					Text = "ZOOM IN";
					OnClick += UILaunchButton_OnClick_ZoomIn;
					break;

				case ButtonStateType.Embark:
					TextColor = new Color(0, 200, 0);
					Text = "Embark";
					OnClick += UILaunchButton_OnClick_Embark;
					break;

				case ButtonStateType.Launch:
					TextColor = new Color(0, 255, 0);
					Text = "LAUNCH";
					OnClick += UILaunchButton_OnClick_Launch;
					break;

				default:
					TextColor = Color.White;
					Text = "DEFAULT";
					CanClick = false;
					break;
			}


			buttonText.TextColor = TextColor;
			buttonText.SetText(Text);

			if (!CanClick)
			{
				buttonPanel.BorderColor = Color.Black;
				buttonPanel.BackgroundColor = new Color(63, 82, 151) * 0.6f;
			}
			else if (IsMouseHovering)
			{
				buttonPanel.BorderColor = Color.Gold;
				buttonPanel.BackgroundColor = new Color(63, 82, 151) * 0.9f;
			}
			else
			{
				buttonPanel.BorderColor = Color.Black;
				buttonPanel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			}

			buttonPanel.BackgroundColor.A = 255;
		}

		private void UILaunchButton_OnClick_ZoomIn(UIMouseEvent evt, UIElement listeningElement)
		{
			ZoomIn(false);
		}

		private void UILaunchButton_OnClick_Embark(UIMouseEvent evt, UIElement listeningElement)
		{
			Embark();
		}

		private void UILaunchButton_OnClick_Launch(UIMouseEvent evt, UIElement listeningElement)
		{
			Launch();
		}
	}
}
