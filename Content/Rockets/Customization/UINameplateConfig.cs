using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Macrocosm.Common.UI;
using System;
using System.Linq;
using Terraria.UI;
using Terraria.ModLoader;
using Macrocosm.Common.DataStructures;

namespace Macrocosm.Content.Rockets.Customization
{
	public class UINameplateConfig : UIPanel
	{
		public Rocket Rocket = new();

		private UIInputTextBox uITextBox;

		public string Text
		{
			get => uITextBox.Text;
			set => uITextBox.SetText(Nameplate.FormatText(value));
		}

		public bool HasFocus
		{
			get => uITextBox.HasFocus;
			set
			{
				uITextBox.HasFocus = value;

				if (value)
					OnFocusGain();
			}
		}
		
		/// <summary> Called when focus is gained </summary>
		public Action OnFocusGain { get; set; } = () => { };

		/// <summary> Called when focus is lost </summary>
		public Action OnFocusLost { get; set; } = () => { };

		UISelectableIconButton alignLeft;
		UISelectableIconButton alignCenterHorizontal;
		UISelectableIconButton alignRight;

		UISelectableIconButton alignTop;
		UISelectableIconButton alignCenterVertical;
		UISelectableIconButton alignBottom;

		public UINameplateConfig()
		{
		}

		public override void OnInitialize()
		{
			Width.Set(0, 0.99f);
			Height.Set(0, 0.08f);
			HAlign = 0.5f;
			BackgroundColor = new Color(53, 72, 135);
			BorderColor = new Color(89, 116, 213, 255);
			SetPadding(0f);

			uITextBox = new(Language.GetText("Mods.Macrocosm.Common.Rocket").Value)
			{
				Width = new(0f, 0.54f),
				Height = new(0f, 0.82f),
				HAlign = 0.02f,
				VAlign = 0.5f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
				HoverBorderColor = Color.Gold,
				TextMaxLenght = Nameplate.MaxChars,
				OnFocusGain = OnFocusGain,
				TextScale = 1.2f,
				FormatText = Nameplate.FormatText
			};
			Append(uITextBox);

			string path = "Macrocosm/Content/Rockets/Textures/Buttons/";

			alignLeft = new(ModContent.Request<Texture2D>(path + "AlignLeft"))
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = StyleDimension.FromPercent(0.56f)
			};
			alignLeft.OnLeftClick += AlignLeft_OnLeftClick;
			Append(alignLeft);

			alignCenterHorizontal = new(ModContent.Request<Texture2D>(path + "AlignCenterHorizontal"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.628f)
			};
			alignCenterHorizontal.OnLeftClick += AlignCenterHorizontal_OnLeftClick;
			Append(alignCenterHorizontal);

			alignRight = new(ModContent.Request<Texture2D>(path + "AlignRight"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.696f)
			};
			alignRight.OnLeftClick += AlignRight_OnLeftClick;
			Append(alignRight);

			alignTop = new(ModContent.Request<Texture2D>(path + "AlignTop"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.78f)
			};
			alignTop.OnLeftClick += AlignTop_OnLeftClick;
			Append(alignTop);

			alignCenterVertical = new(ModContent.Request<Texture2D>(path + "AlignCenterVertical"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.848f)
			};
			alignCenterVertical.OnLeftClick += AlignCenterVertical_OnLeftClick;
			Append(alignCenterVertical);

			alignBottom = new(ModContent.Request<Texture2D>(path + "AlignBottom"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.917f)
			};
			alignBottom.OnLeftClick += AlignBottom_OnLeftClick;
			Append(alignBottom);

		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (HasFocus)
			{
				Rocket.CustomizationDummy.Nameplate.Text = Text;
				BorderColor = Color.Gold;
			}
			else
			{
				Text = Rocket.CustomizationDummy.AssignedName;
				BorderColor = new Color(89, 116, 213, 255);
			}

			alignCenterHorizontal.Selected = false; 
			alignCenterVertical.Selected = false;
			alignBottom.Selected = false;
			alignRight.Selected = false;
			alignLeft.Selected = false;
			alignTop.Selected = false;

			switch(Rocket.CustomizationDummy.Nameplate.HorizontalAlignment)
			{
				case TextAlignmentHorizontal.Left: alignLeft.Selected = true; break;
				case TextAlignmentHorizontal.Right: alignRight.Selected = true; break;
				case TextAlignmentHorizontal.Center: alignCenterHorizontal.Selected = true; break;
			}

			switch (Rocket.CustomizationDummy.Nameplate.VerticalAlignment)
			{
				case TextAlignmentVertical.Top: alignTop.Selected = true; break;
				case TextAlignmentVertical.Bottom: alignBottom.Selected = true; break;
				case TextAlignmentVertical.Center: alignCenterVertical.Selected = true; break;
			}
		}

		private void AlignLeft_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Left;
		}

		private void AlignCenterHorizontal_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Center;
		}

		private void AlignRight_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Right;
		}

		private void AlignTop_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Top;
		}

		private void AlignCenterVertical_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Center;
		}

		private void AlignBottom_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Bottom;
		}
	}
}
