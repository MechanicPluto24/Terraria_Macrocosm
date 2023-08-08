using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Macrocosm.Common.UI;
using System;
using System.Linq;
using Terraria.UI;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Customization
{
	public class UINameplateConfig : UIPanel
	{
		public Rocket RocketDummy = new();

		private UIInputTextBox uITextBox;

		public string Text => uITextBox.Text;
		public bool HasFocus
		{
			get => uITextBox.HasFocus;
			set => uITextBox.HasFocus = value;
		}

		public void SetText(string text) => uITextBox.SetText(text);	

		public Action OnFocusSet = () => { };
		public Func<bool> CheckTextSubmit = () => false;

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
				Width = new(0f, 0.43f),
				Height = new(0f, 0.82f),
				HAlign = 0.02f,
				VAlign = 0.5f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
				HoverBorderColor = Color.Gold,
				TextMaxLenght = Nameplate.MaxChars,
				OnFocusSet = OnFocusSet,
				CheckTextSubmit = CheckTextSubmit,
				TextScale = 1.2f,
				FormatText = (text) => new(text.ToUpperInvariant().Where(Nameplate.SupportedCharacters.Contains).ToArray())
			};
			Append(uITextBox);

			string path = "Macrocosm/Content/Rockets/Textures/Buttons/";

			alignLeft = new(ModContent.Request<Texture2D>(path + "AlignLeft"))
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = StyleDimension.FromPercent(0.45f)
			};
			alignLeft.OnLeftClick += AlignLeft_OnLeftClick;
			Append(alignLeft);

			alignCenterHorizontal = new(ModContent.Request<Texture2D>(path + "AlignCenterHorizontal"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.52f)
			};
			alignCenterHorizontal.OnLeftClick += AlignCenterHorizontal_OnLeftClick;
			Append(alignCenterHorizontal);

			alignRight = new(ModContent.Request<Texture2D>(path + "AlignRight"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.592f)
			};
			alignRight.OnLeftClick += AlignRight_OnLeftClick;
			Append(alignRight);

			alignTop = new(ModContent.Request<Texture2D>(path + "AlignTop"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.68f)
			};
			alignTop.OnLeftClick += AlignTop_OnLeftClick;
			Append(alignTop);

			alignCenterVertical = new(ModContent.Request<Texture2D>(path + "AlignCenterVertical"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.752f)
			};
			alignCenterVertical.OnLeftClick += AlignCenterVertical_OnLeftClick;
			Append(alignCenterVertical);

			alignBottom = new(ModContent.Request<Texture2D>(path + "AlignBottom"))
			{
				VAlign = 0.5f,
				Left = StyleDimension.FromPercent(0.825f)
			};
			alignBottom.OnLeftClick += AlignBottom_OnLeftClick;
			Append(alignBottom);

		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (HasFocus)
				BorderColor = Color.Gold;
			else
				BorderColor = new Color(89, 116, 213, 255);


			alignCenterHorizontal.Selected = false; 
			alignCenterVertical.Selected = false;
			alignBottom.Selected = false;
			alignRight.Selected = false;
			alignLeft.Selected = false;
			alignTop.Selected = false;

			switch(RocketDummy.EngineModule.Nameplate.HorizontalAlignment)
			{
				case Nameplate.AlignmentModeHorizontal.Left: alignLeft.Selected = true; break;
				case Nameplate.AlignmentModeHorizontal.Right:alignRight.Selected = true; break;
				case Nameplate.AlignmentModeHorizontal.Center: alignCenterHorizontal.Selected = true; break;
			}

			switch (RocketDummy.EngineModule.Nameplate.VerticalAlignment)
			{
				case Nameplate.AlignmentModeVertical.Top: alignTop.Selected = true; break;
				case Nameplate.AlignmentModeVertical.Bottom: alignBottom.Selected = true; break;
				case Nameplate.AlignmentModeVertical.Center: alignCenterVertical.Selected = true; break;
			}
		}

		private void AlignLeft_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			OnFocusSet();
			RocketDummy.EngineModule.Nameplate.HorizontalAlignment = Nameplate.AlignmentModeHorizontal.Left;
		}

		private void AlignCenterHorizontal_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			OnFocusSet();
			RocketDummy.EngineModule.Nameplate.HorizontalAlignment = Nameplate.AlignmentModeHorizontal.Center;
		}

		private void AlignRight_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			OnFocusSet();
			RocketDummy.EngineModule.Nameplate.HorizontalAlignment = Nameplate.AlignmentModeHorizontal.Right;
		}

		private void AlignTop_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			OnFocusSet();
			RocketDummy.EngineModule.Nameplate.VerticalAlignment = Nameplate.AlignmentModeVertical.Top;
		}

		private void AlignCenterVertical_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			OnFocusSet();
			RocketDummy.EngineModule.Nameplate.VerticalAlignment = Nameplate.AlignmentModeVertical.Center;
		}

		private void AlignBottom_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			HasFocus = true;
			OnFocusSet();
			RocketDummy.EngineModule.Nameplate.VerticalAlignment = Nameplate.AlignmentModeVertical.Bottom;
		}
	}
}
