using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Common.DataStructures;
using Terraria.Localization;
using Terraria.UI;
using System;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UICustomizationTab : UIPanel, ITabUIElement, IRocketDataConsumer
    {
        public Rocket Rocket { get; set; } 

		private UIPanel RocketPreviewBackground;
		private UIRocketPreviewLarge RocketPreview;

		private UIPanel CustomizationPanelBackground;

		private UIPanel modulePicker;
		private UIText modulePickerTitle;
		private string currentModuleName = "CommandPod";

		private UIPanel CustomizationControlPanel;
		private UIPanelIconButton applyButton;
		private UIPanelIconButton cancelButton;
		private UIPanelIconButton resetModuleButton;
		private UIPanelIconButton resetRocketButton;

		private UIPanel nameplateConfigPanel;
		private UIInputTextBox nameplateTextBox;
		private UISelectableIconButton nameplateColorPicker;
		private UISelectableIconButton alignLeft;
		private UISelectableIconButton alignCenterHorizontal;
		private UISelectableIconButton alignRight;
		private UISelectableIconButton alignTop;
		private UISelectableIconButton alignCenterVertical;
		private UISelectableIconButton alignBottom;

		public UIPanel DetailConfig;
		public UIPanel PatternConfig;

		public UIColorMenuHSL HSLMenu;

		public UICustomizationTab()
		{
		}

		public override void OnInitialize()
        {
			Width.Set(0, 1f);
			Height.Set(0, 1f);
			HAlign = 0.5f;
			VAlign = 0.5f;

			SetPadding(6f);

			BackgroundColor = new Color(13, 23, 59, 127);
			BorderColor = new Color(15, 15, 15, 255);

			RocketPreviewBackground = new()
			{
				Width = new(0, 0.4f),
				Height = new(0, 1f),
				Left = new (0,0.605f),
				HAlign = 0f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			RocketPreviewBackground.SetPadding(2f);
			RocketPreviewBackground.OverflowHidden = true;
			RocketPreviewBackground.Activate();
			Append(RocketPreviewBackground);

			RocketPreview = new();
			RocketPreviewBackground.Append(RocketPreview);

			CustomizationPanelBackground = new()
			{
				Width = new(0, 0.6f),
				Height = new(0, 1f),
				HAlign = 0f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			CustomizationPanelBackground.SetPadding(6f);
			Append(CustomizationPanelBackground);

			modulePicker = CreateModulePicker();
			CustomizationPanelBackground.Append(modulePicker);

			nameplateConfigPanel = CreateNameplateConfigPanel();
			CustomizationPanelBackground.Append(nameplateConfigPanel);

			DetailConfig = CreateDetailConfigPanel();
			CustomizationPanelBackground.Append(DetailConfig);

			PatternConfig = CreatePatternConfigPanel();
			CustomizationPanelBackground.Append(PatternConfig);
			 
			CustomizationControlPanel = CreateControlPanel();
			CustomizationPanelBackground.Append(CustomizationControlPanel);

			HSLMenu = new()
			{
				HAlign = 0.98f,
				Top = new(0f, 0.092f)
			};

			HSLMenu.SetupApplyAndCancelButtons(() => nameplateColorPicker.Selected = false, () => nameplateColorPicker.Selected = false);

			CustomizationPanelBackground.Activate();
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			RocketPreview.Rocket = Rocket;

			modulePickerTitle.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules." + currentModuleName));

			if (nameplateTextBox.HasFocus)
				Rocket.CustomizationDummy.Nameplate.Text = nameplateTextBox.Text;
			else
				nameplateTextBox.Text = Rocket.CustomizationDummy.AssignedName;

			if (nameplateColorPicker.Selected)
			{
				Rocket.CustomizationDummy.Nameplate.TextColor = HSLMenu.PendingColor;
				nameplateColorPicker.BackPanelColor = HSLMenu.PendingColor;
				//nameplateTextBox.TextColor = HSLMenuProvider.PendingColor; // I don't think we want colored text in the text box lol -- Feldy
			} 
			else
			{
				nameplateColorPicker.BackPanelColor = Rocket.CustomizationDummy.Nameplate.TextColor;
				//nameplateTextBox.TextColor = Rocket.CustomizationDummy.Nameplate.TextColor;
			}

			alignCenterHorizontal.Selected = false;
			alignCenterVertical.Selected = false;
			alignBottom.Selected = false;
			alignRight.Selected = false;
			alignLeft.Selected = false;
			alignTop.Selected = false;

			switch (Rocket.CustomizationDummy.Nameplate.HorizontalAlignment)
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

		public void OnTabClose()
		{
			NameplateElementsLoseFocus();
		}

		private void ApplyCustomizationChanges()
		{
			NameplateElementsLoseFocus();
			Rocket.ApplyCustomizationChanges();
		}

		private void CancelCustomizationChanges()
		{
			NameplateElementsLoseFocus();
			Rocket.RefreshCustomizationDummy();
		}

		private void ResetRocketToDefaults()
		{
			NameplateElementsLoseFocus();
			Rocket.ResetCustomizationDummyToDefault();
		}

		private void ResetCurrentModuleToDefaults()
		{
			NameplateElementsLoseFocus();
			Rocket.ResetDummyModuleToDefault(currentModuleName);
		}

		private void NameplateTextOnFocusGain()
		{
			JumpToModule("EngineModule");
			nameplateColorPicker.Selected = false;
		}

		private void NameplateColorPickerOnSelected()
		{
			JumpToModule("EngineModule");

			HSLMenu.SetColorHSL(Rocket.CustomizationDummy.Nameplate.TextColor.ToHSL());
			HSLMenu.CaptureCurrentColor();
			CustomizationPanelBackground.ReplaceChildWith(CustomizationControlPanel, HSLMenu);

			nameplateTextBox.HasFocus = false;
		}

		private void NameplateColorPickerOnDeselected()
		{
			Rocket.CustomizationDummy.Nameplate.TextColor = HSLMenu.PreviousColor;
			CustomizationPanelBackground.ReplaceChildWith(HSLMenu, CustomizationControlPanel);
		}

		private void JumpToModule(string moduleName)
		{
			RocketPreview.UpdateModule(moduleName);
			currentModuleName = moduleName;
		}

		private void NameplateElementsLoseFocus()
		{
			nameplateTextBox.HasFocus = false;
			nameplateColorPicker.Selected = false;
		}

		private void NextModule()
		{
			if (RocketPreview.AnimationActive)
				return;

			if (RocketPreview.CurrentModuleIndex == Rocket.Modules.Count - 1)
				RocketPreview.UpdateModule(0);
			else
				RocketPreview.UpdateModule(RocketPreview.CurrentModuleIndex + 1);

			currentModuleName = RocketPreview.CurrentModuleName;

			NameplateElementsLoseFocus();
		}

		private void PreviousModule()
		{
			if (RocketPreview.AnimationActive)
				return;

			if (RocketPreview.CurrentModuleIndex == 0)
				RocketPreview.UpdateModule(Rocket.Modules.Count - 1);
			else
				RocketPreview.UpdateModule(RocketPreview.CurrentModuleIndex - 1);

			currentModuleName = RocketPreview.CurrentModuleName;

			NameplateElementsLoseFocus();
		}

		private const string buttonsPath = "Macrocosm/Content/Rockets/Textures/Buttons/";
		private const string symbolsPath = "Macrocosm/Content/Rockets/Textures/Symbols/";

		private UIPanel CreateModulePicker()
		{
			var mode = ReLogic.Content.AssetRequestMode.ImmediateLoad;

			modulePicker = new()
			{
				Width = new(0, 0.36f),
				Height = new(0, 0.25f),
				HAlign = 0.007f,
				Top = new(0f, 0.092f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
			};
			modulePicker.SetPadding(0f);

			modulePickerTitle = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules." + currentModuleName), 0.9f, false)
			{
				IsWrapped = false,
				HAlign = 0.5f,
				VAlign = 0.05f,
				TextColor = Color.White
			};
			modulePicker.Append(modulePickerTitle);

			UIPanel moduleIconPreviewPanel = new()
			{ 
				Width = new(0f, 0.6f),
				Height = new(0f, 0.7f),
				HAlign = 0.5f,
				VAlign = 0.6f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			modulePicker.Append(moduleIconPreviewPanel);

			UIHoverImageButton leftButton = new(ModContent.Request<Texture2D>(buttonsPath + "BackArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "BackArrowBorder", mode))
			{
				VAlign = 0.5f,
				Left = new StyleDimension(0f, 0f),
			};
			leftButton.SetVisibility(1f, 1f, 1f);
			leftButton.CheckInteractible = () => !RocketPreview.AnimationActive;
			leftButton.OnLeftClick += (_, _) => PreviousModule();
			modulePicker.Append(leftButton);

			UIHoverImageButton rightButton = new(ModContent.Request<Texture2D>(buttonsPath + "ForwardArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "ForwardArrowBorder", mode))
			{
				VAlign = 0.5f,
				Left = new StyleDimension(0, 0.79f),
			};
			rightButton.SetVisibility(1f, 1f, 1f);
			rightButton.CheckInteractible = () => !RocketPreview.AnimationActive;
			rightButton.OnLeftClick += (_, _) => NextModule();
			modulePicker.Append(rightButton);

			return modulePicker;
		}

		private UIPanel CreateControlPanel()
		{
			CustomizationControlPanel = new()
			{
				Width = new(0f, 0.62f),
				Height = new(0, 0.25f),
				HAlign = 0.98f,
				Top = new(0f, 0.092f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			CustomizationControlPanel.SetPadding(2f);
			CustomizationPanelBackground.Append(CustomizationControlPanel);

			resetRocketButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetRed"))
			{
				VAlign = 0.9f,
				Left = new(0f, 0.22f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.ResetRocket")

			};
			resetRocketButton.OnLeftClick += (_, _) => ResetRocketToDefaults();
			CustomizationControlPanel.Append(resetRocketButton);

			resetModuleButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetGray"))
			{
				VAlign = 0.9f,
				Left = new(0f, 0.34f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.ResetModule")
			};
			resetModuleButton.OnLeftClick += (_, _) => ResetCurrentModuleToDefaults();
			CustomizationControlPanel.Append(resetModuleButton);

			cancelButton = new(ModContent.Request<Texture2D>(symbolsPath + "CrossmarkRed")) 
			{
				VAlign = 0.9f,
				Left = new(0f, 0.46f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.CustomizationCancel")
			};
			cancelButton.OnLeftClick += (_, _) => CancelCustomizationChanges();
			CustomizationControlPanel.Append(cancelButton);

			applyButton = new(ModContent.Request<Texture2D>(symbolsPath + "CheckmarkGreen"))
			{
				VAlign = 0.9f,
				Left = new(0f, 0.58f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.CustomizationApply")
			};
			applyButton.OnLeftClick += (_, _) => ApplyCustomizationChanges();
			CustomizationControlPanel.Append(applyButton);
			return CustomizationControlPanel;
		}

		private UIPanel CreateNameplateConfigPanel()
		{
			nameplateConfigPanel = new()
			{
				Width = new StyleDimension(0, 0.99f),
				Height = new StyleDimension(0, 0.08f),
				HAlign = 0.5f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
			};
			nameplateConfigPanel.SetPadding(0f);

			nameplateTextBox = new(Language.GetText("Mods.Macrocosm.Common.Rocket").Value)
			{
				Width = new(0f, 0.46f),
				Height = new(0f, 0.74f),
				HAlign = 0.02f,
				VAlign = 0.55f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
				HoverBorderColor = Color.Gold,
				TextMaxLenght = Nameplate.MaxChars,
				TextScale = 1f,
				FormatText = Nameplate.FormatText,
				OnFocusGain = NameplateTextOnFocusGain

			};
			nameplateConfigPanel.Append(nameplateTextBox);

			nameplateColorPicker = new()
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = new(0f, 0.482f)
			};

			nameplateColorPicker.OnLeftClick += (_, _) => { nameplateColorPicker.Selected = true; };
			nameplateColorPicker.OnSelected = NameplateColorPickerOnSelected;

			nameplateColorPicker.OnRightClick += (_, _) => { nameplateColorPicker.Selected = false; };
			nameplateColorPicker.OnDeselected += NameplateColorPickerOnDeselected;
	
			nameplateConfigPanel.Append(nameplateColorPicker);

			alignLeft = new(ModContent.Request<Texture2D>(symbolsPath + "AlignLeft"))
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = new(0f, 0.56f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignLeft")
			};

			alignLeft.OnLeftClick += (_, _) =>
			{
				JumpToModule("EngineModule");
				Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Left;
			};
			nameplateConfigPanel.Append(alignLeft);

			alignCenterHorizontal = new(ModContent.Request<Texture2D>(symbolsPath + "AlignCenterHorizontal"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.628f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignCenterHorizontal")

			};
			alignCenterHorizontal.OnLeftClick += (_, _) =>
			{
				JumpToModule("EngineModule");
				Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Center;
			};
			nameplateConfigPanel.Append(alignCenterHorizontal);

			alignRight = new(ModContent.Request<Texture2D>(symbolsPath + "AlignRight"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.696f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignRight")
			};
			alignRight.OnLeftClick += (_, _) =>
			{
				JumpToModule("EngineModule");
				Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Right;
			};
			nameplateConfigPanel.Append(alignRight);

			alignTop = new(ModContent.Request<Texture2D>(symbolsPath + "AlignTop"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.78f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignTop")
			};
			alignTop.OnLeftClick += (_, _) =>
			{
				JumpToModule("EngineModule");
				Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Top;
			};
			nameplateConfigPanel.Append(alignTop);

			alignCenterVertical = new(ModContent.Request<Texture2D>(symbolsPath + "AlignCenterVertical"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.848f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignCenterVertical")
			};
			alignCenterVertical.OnLeftClick += (_, _) =>
			{
				JumpToModule("EngineModule");
				Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Center;
			};
			nameplateConfigPanel.Append(alignCenterVertical);

			alignBottom = new(ModContent.Request<Texture2D>(symbolsPath + "AlignBottom"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.917f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignBottom")
			};
			alignBottom.OnLeftClick += (_, _) =>
			{
				JumpToModule("EngineModule");
				Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Bottom;
			};
			nameplateConfigPanel.Append(alignBottom);

			return nameplateConfigPanel;
		}

		private UIPanel CreateDetailConfigPanel()
		{
			UIPanel detailConfigPanel = new()
			{
				Width = new(0, 0.99f),
				Height = new(0, 0.4f),
				HAlign = 0.5f,
				Top = new(0f, 0.595f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
			};
			detailConfigPanel.SetPadding(0f);
			return detailConfigPanel;
		}

		private UIPanel CreatePatternConfigPanel()
		{
			UIPanel patternConfigPanel = new()
			{
				Width = new(0, 0.99f),
				Height = new(0, 0.22f),
				HAlign = 0.5f,
				Top = new(0f, 0.36f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			patternConfigPanel.SetPadding(0f);
			return patternConfigPanel;
		}
	}
}
