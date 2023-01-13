using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
{
	public class UILaunchButton : UIElement
	{
		public delegate void OnClick_ZoomIn(bool useDefault);
		public OnClick_ZoomIn ZoomIn = (useDefault) => { };

		public delegate void OnClick_Launch();
		public OnClick_Launch Launch = () => { };

		public bool CanClick;

		private UIText buttonText;
		private UIPanel buttonPanel;

		public enum StateType
		{
			NoTarget,
			CantReach,
			AlreadyHere,
			ZoomIn,
			Launch
		}
		public StateType ButtonState;
		
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
			OnClick -= UILaunchButton_OnClick_Launch;
			OnClick -= UILaunchButton_OnClick_ZoomIn;
			CanClick = true;

			string text = "";
			Color textColor = Color.White;
			float textScale = 0.9f;

			switch (ButtonState)
			{
				case StateType.NoTarget:
					textColor = Color.Gold;
					text = "NO TARGET";
					CanClick = false;
					break;

				case StateType.CantReach:
					textColor = Color.Red;
					textScale = 0.75f;
					text = "INACCESSIBLE";
					CanClick = false;
					break;

				case StateType.AlreadyHere:
					textColor = Color.Gray * 1.3f;
					textScale = 0.58f;
					text = "CURRENT LOCATION";
					CanClick = false;
					break;

				case StateType.ZoomIn:
					textColor = Color.White;
					text = "ZOOM IN";
					textScale = 1.05f;
					OnClick += UILaunchButton_OnClick_ZoomIn;
					break;

				case StateType.Launch:
					textColor = new Color(0, 255, 0);
					textScale = 1.1f;
					text = "LAUNCH";
					OnClick += UILaunchButton_OnClick_Launch;
					break;

				default:
					CanClick = false;
					break;
			}


			buttonText.TextColor = textColor;
			buttonText.SetText(text, textScale, true);

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


		private void UILaunchButton_OnClick_Launch(UIMouseEvent evt, UIElement listeningElement)
		{
			Launch();
		}
	}
}
