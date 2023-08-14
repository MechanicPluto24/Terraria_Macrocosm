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

		private UIPanel rocketPreviewBackground;
		private UIRocketPreviewLarge rocketPreview;

		private UIPanel customizationPanelBackground;

		private UIPanel modulePicker;
		private UIText modulePickerTitle;
		private string currentModuleName = "CommandPod";

		private UIPanel customizationControlPanel;
		private UIPanelIconButton applyButton;
		private UIPanelIconButton cancelButton;
		private UIPanelIconButton resetModuleButton;
		private UIPanelIconButton resetRocketButton;

		private UIPanel nameplateConfigPanel;
		private UIInputTextBox nameplateTextBox;
		private UIFocusIconButton nameplateColorPicker;
		private UIFocusIconButton alignLeft;
		private UIFocusIconButton alignCenterHorizontal;
		private UIFocusIconButton alignRight;
		private UIFocusIconButton alignTop;
		private UIFocusIconButton alignCenterVertical;
		private UIFocusIconButton alignBottom;

		private UIPanel detailConfig;
		private UIPanel patternConfig;

		private UIColorMenuHSL hslMenu;

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

			rocketPreviewBackground = new()
			{
				Width = new(0, 0.4f),
				Height = new(0, 1f),
				Left = new (0,0.605f),
				HAlign = 0f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			rocketPreviewBackground.SetPadding(2f);
			rocketPreviewBackground.OverflowHidden = true;
			rocketPreviewBackground.Activate();
			Append(rocketPreviewBackground);

			rocketPreview = new();
			rocketPreviewBackground.Append(rocketPreview);

			customizationPanelBackground = new()
			{
				Width = new(0, 0.6f),
				Height = new(0, 1f),
				HAlign = 0f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			customizationPanelBackground.SetPadding(6f);
			Append(customizationPanelBackground);

			modulePicker = CreateModulePicker();
			customizationPanelBackground.Append(modulePicker);

			nameplateConfigPanel = CreateNameplateConfigPanel();
			customizationPanelBackground.Append(nameplateConfigPanel);

			detailConfig = CreateDetailConfigPanel();
			customizationPanelBackground.Append(detailConfig);

			patternConfig = CreatePatternConfigPanel();
			customizationPanelBackground.Append(patternConfig);
			 
			customizationControlPanel = CreateControlPanel();
			customizationPanelBackground.Append(customizationControlPanel);

			hslMenu = new()
			{
				HAlign = 0.98f,
				Top = new(0f, 0.092f)
			};

			hslMenu.SetupApplyAndCancelButtons(() => nameplateColorPicker.HasFocus = false, () => nameplateColorPicker.HasFocus = false);

			customizationPanelBackground.Activate();
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			modulePickerTitle.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules." + currentModuleName));

			if (nameplateTextBox.HasFocus)
				Rocket.CustomizationDummy.Nameplate.Text = nameplateTextBox.Text;
			else
				nameplateTextBox.Text = Rocket.CustomizationDummy.AssignedName;

			if (nameplateColorPicker.HasFocus)
			{
				Rocket.CustomizationDummy.Nameplate.TextColor = hslMenu.PendingColor;
				nameplateColorPicker.BackPanelColor = hslMenu.PendingColor;
				//nameplateTextBox.TextColor = HSLMenuProvider.PendingColor; // I don't think we want colored text in the text box lol -- Feldy
			} 
			else
			{
				nameplateColorPicker.BackPanelColor = Rocket.CustomizationDummy.Nameplate.TextColor;
				//nameplateTextBox.TextColor = Rocket.CustomizationDummy.Nameplate.TextColor;
			}

			switch (Rocket.CustomizationDummy.Nameplate.HorizontalAlignment)
			{
				case TextAlignmentHorizontal.Left: alignLeft.HasFocus = true; break;
				case TextAlignmentHorizontal.Right: alignRight.HasFocus = true; break;
				case TextAlignmentHorizontal.Center: alignCenterHorizontal.HasFocus = true; break;
			}

			switch (Rocket.CustomizationDummy.Nameplate.VerticalAlignment)
			{
				case TextAlignmentVertical.Top: alignTop.HasFocus = true; break;
				case TextAlignmentVertical.Bottom: alignBottom.HasFocus = true; break;
				case TextAlignmentVertical.Center: alignCenterVertical.HasFocus = true; break;
			}

			if(nameplateColorPicker.HasFocus)
				hslMenu.CaptureKeyboard();
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
		}

		private void NameplateColorPickerOnFocusGain()
		{
			JumpToModule("EngineModule");

			hslMenu.SetColorHSL(Rocket.CustomizationDummy.Nameplate.TextColor.ToHSL());
			hslMenu.CaptureCurrentColor();
			customizationPanelBackground.ReplaceChildWith(customizationControlPanel, hslMenu);
		}

		private void NameplateColorPickerOnFocusLost()
		{
			Rocket.CustomizationDummy.Nameplate.TextColor = hslMenu.PreviousColor;
			customizationPanelBackground.ReplaceChildWith(hslMenu, customizationControlPanel);
		}

		private void JumpToModule(string moduleName)
		{
			rocketPreview.UpdateModule(moduleName);
			currentModuleName = moduleName;
		}

		private void NameplateElementsLoseFocus()
		{
			nameplateTextBox.HasFocus = false;
			nameplateColorPicker.HasFocus = false;
		}

		private void NextModule()
		{
			if (rocketPreview.AnimationActive)
				return;

			if (rocketPreview.CurrentModuleIndex == Rocket.Modules.Count - 1)
				rocketPreview.UpdateModule(0);
			else
				rocketPreview.UpdateModule(rocketPreview.CurrentModuleIndex + 1);

			currentModuleName = rocketPreview.CurrentModuleName;

			NameplateElementsLoseFocus();
		}

		private void PreviousModule()
		{
			if (rocketPreview.AnimationActive)
				return;

			if (rocketPreview.CurrentModuleIndex == 0)
				rocketPreview.UpdateModule(Rocket.Modules.Count - 1);
			else
				rocketPreview.UpdateModule(rocketPreview.CurrentModuleIndex - 1);

			currentModuleName = rocketPreview.CurrentModuleName;

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
			leftButton.CheckInteractible = () => !rocketPreview.AnimationActive;
			leftButton.OnLeftClick += (_, _) => PreviousModule();
			modulePicker.Append(leftButton);

			UIHoverImageButton rightButton = new(ModContent.Request<Texture2D>(buttonsPath + "ForwardArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "ForwardArrowBorder", mode))
			{
				VAlign = 0.5f,
				Left = new StyleDimension(0, 0.79f),
			};
			rightButton.SetVisibility(1f, 1f, 1f);
			rightButton.CheckInteractible = () => !rocketPreview.AnimationActive;
			rightButton.OnLeftClick += (_, _) => NextModule();
			modulePicker.Append(rightButton);

			return modulePicker;
		}

		private UIPanel CreateControlPanel()
		{
			customizationControlPanel = new()
			{
				Width = new(0f, 0.62f),
				Height = new(0, 0.25f),
				HAlign = 0.98f,
				Top = new(0f, 0.092f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			customizationControlPanel.SetPadding(2f);
			customizationPanelBackground.Append(customizationControlPanel);

			resetRocketButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetRed"))
			{
				VAlign = 0.9f,
				Left = new(0f, 0.22f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.ResetRocket")

			};
			resetRocketButton.OnLeftClick += (_, _) => ResetRocketToDefaults();
			customizationControlPanel.Append(resetRocketButton);

			resetModuleButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetGray"))
			{
				VAlign = 0.9f,
				Left = new(0f, 0.34f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.ResetModule")
			};
			resetModuleButton.OnLeftClick += (_, _) => ResetCurrentModuleToDefaults();
			customizationControlPanel.Append(resetModuleButton);

			cancelButton = new(ModContent.Request<Texture2D>(symbolsPath + "CrossmarkRed")) 
			{
				VAlign = 0.9f,
				Left = new(0f, 0.46f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.CustomizationCancel")
			};
			cancelButton.OnLeftClick += (_, _) => CancelCustomizationChanges();
			customizationControlPanel.Append(cancelButton);

			applyButton = new(ModContent.Request<Texture2D>(symbolsPath + "CheckmarkGreen"))
			{
				VAlign = 0.9f,
				Left = new(0f, 0.58f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.CustomizationApply")
			};
			applyButton.OnLeftClick += (_, _) => ApplyCustomizationChanges();
			customizationControlPanel.Append(applyButton);
			return customizationControlPanel;
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
				FocusContext = "NameplateEdit",
				OnFocusGain = NameplateTextOnFocusGain

			};
			nameplateConfigPanel.Append(nameplateTextBox);

			nameplateColorPicker = new()
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = new(0f, 0.482f),
				FocusContext = "NameplateEdit"
			};

			nameplateColorPicker.OnLeftClick += (_, _) => { nameplateColorPicker.HasFocus = true; };
			nameplateColorPicker.OnFocusGain = NameplateColorPickerOnFocusGain;

			nameplateColorPicker.OnRightClick += (_, _) => { nameplateColorPicker.HasFocus = false; };
			nameplateColorPicker.OnFocusLost += NameplateColorPickerOnFocusLost;
	
			nameplateConfigPanel.Append(nameplateColorPicker);

			alignLeft = new(ModContent.Request<Texture2D>(symbolsPath + "AlignLeft"))
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = new(0f, 0.56f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignLeft"),
				FocusContext = "HorizontalAlignment",
				OnFocusGain = () =>
				{
					JumpToModule("EngineModule");
					Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Left;
				}
			};
			alignLeft.OnLeftClick += (_, _) => alignLeft.HasFocus = true;
			 
			nameplateConfigPanel.Append(alignLeft);

			alignCenterHorizontal = new(ModContent.Request<Texture2D>(symbolsPath + "AlignCenterHorizontal"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.628f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignCenterHorizontal"),
				FocusContext = "HorizontalAlignment",
				OnFocusGain = () =>
				{
					JumpToModule("EngineModule");
					Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Center;
				}
			};
			alignCenterHorizontal.OnLeftClick += (_, _) => alignCenterHorizontal.HasFocus = true;
			nameplateConfigPanel.Append(alignCenterHorizontal);

			alignRight = new(ModContent.Request<Texture2D>(symbolsPath + "AlignRight"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.696f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignRight"),
				FocusContext = "HorizontalAlignment",
				OnFocusGain = () =>
				{
					JumpToModule("EngineModule");
					Rocket.CustomizationDummy.Nameplate.HorizontalAlignment = TextAlignmentHorizontal.Right;
				}
			};
			alignRight.OnLeftClick += (_, _) => alignRight.HasFocus = true;
			nameplateConfigPanel.Append(alignRight);

			alignTop = new(ModContent.Request<Texture2D>(symbolsPath + "AlignTop"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.78f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignTop"),
				FocusContext = "VerticalAlignment",
				OnFocusGain = () =>
				{
					JumpToModule("EngineModule");
					Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Top;
				}
			};
			alignTop.OnLeftClick += (_, _) => alignTop.HasFocus = true;
			nameplateConfigPanel.Append(alignTop);

			alignCenterVertical = new(ModContent.Request<Texture2D>(symbolsPath + "AlignCenterVertical"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.848f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignCenterVertical"),
				FocusContext = "VerticalAlignment",
				OnFocusGain = () =>
				{
					JumpToModule("EngineModule");
					Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Center;
				}
			};
			alignCenterVertical.OnLeftClick += (_, _) => alignCenterVertical.HasFocus = true;
			nameplateConfigPanel.Append(alignCenterVertical);

			alignBottom = new(ModContent.Request<Texture2D>(symbolsPath + "AlignBottom"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.917f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignBottom"),
				FocusContext = "VerticalAlignment",
				OnFocusGain = () =>
				{
					JumpToModule("EngineModule");
					Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Bottom;
				}
			};
			alignBottom.OnLeftClick += (_, _) => alignBottom.HasFocus = true;
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
