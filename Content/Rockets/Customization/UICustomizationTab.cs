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
using System.Linq;
using Terraria;
using System.Collections.Generic;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework.Input;

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
		private UIHoverImageButton leftButton;
		private UIHoverImageButton rightButton;
		private string currentModuleName = "CommandPod";
		private RocketModule currentModule;

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

		private UIPanel detailConfigPanel;

		private UIPanel patternConfigPanel;
		private UIListScrollablePanel patternSelector;
		private UIPatternIcon currentPatternIcon;

		private UIPanelIconButton resetPatternButton;
		private UIVerticalSeparator colorPickerSeparator;
		private List<(UIFocusIconButton picker, int colorIndex)> patternColorPickers;
		
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

			rocketPreview = new()
			{
				OnModuleChange = OnCurrentModuleChange
			};
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

			detailConfigPanel = CreateDetailConfigPanel();
			customizationPanelBackground.Append(detailConfigPanel);

			patternConfigPanel = CreatePatternConfigPanel();
			customizationPanelBackground.Append(patternConfigPanel);
			 
			customizationControlPanel = CreateControlPanel();
			customizationPanelBackground.Append(customizationControlPanel);

			hslMenu = new()
			{
				HAlign = 0.98f,
				Top = new(0f, 0.092f)
			};
			hslMenu.SetupApplyAndCancelButtons(ColorPickersLoseFocus, OnHSLMenuCancel);

			customizationPanelBackground.Activate();
		}

		public void OnTabClose()
		{
			Main.blockInput = false;
			AllLoseFocus();
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			UpdateCurrentModule();

			UpdatePatternConfig();
			UpdatePatternColorPickers();

			UpdateNamplateTextBox();
			UpdateNameplateColorPicker();
			UpdateNameplateAlignButtons();

			UpdateHSLMenuVisibility();

			UpdateKeyboardCapture();
		}

		#region Update methods
		private void UpdateCurrentModule()
		{
			currentModule = Rocket.CustomizationDummy.Modules[currentModuleName];
			modulePickerTitle.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules." + currentModuleName));
		}

		private void UpdatePatternConfig()
		{
			Pattern currentPattern = Rocket.CustomizationDummy.Modules[currentModuleName].Pattern;

			if (patternSelector.Any())
			{
				currentPatternIcon = patternSelector.OfType<UIPatternIcon>()
					.Where((icon) => icon.Pattern.Name == currentPattern.Name)
					.FirstOrDefault();

				if (currentPatternIcon is not null)
				{
					currentPatternIcon.HasFocus = true;

					if (patternColorPickers is null)
						CreatePatternColorPickers();
				}
			}
		}

		private void UpdatePatternColorPickers()
		{
			foreach (var (picker, colorIndex) in patternColorPickers)
			{
				if (picker.HasFocus)
				{
					currentPatternIcon.Pattern.SetColor(colorIndex, hslMenu.PendingColor);
					currentModule.Pattern.SetColor(colorIndex, hslMenu.PendingColor);
				}

				picker.BackPanelColor = currentModule.Pattern.GetColor(colorIndex);
			}
		}

		private void UpdateNamplateTextBox()
		{
			if (nameplateTextBox.HasFocus)
				Rocket.CustomizationDummy.Nameplate.Text = nameplateTextBox.Text;
			else
				nameplateTextBox.Text = Rocket.CustomizationDummy.AssignedName;
		}

		private void UpdateNameplateColorPicker()
		{
			if (nameplateColorPicker.HasFocus)
			{
				Rocket.CustomizationDummy.Nameplate.TextColor = hslMenu.PendingColor;
				nameplateColorPicker.BackPanelColor = hslMenu.PendingColor;
				//nameplateTextBox.TextColor = hslMenu.PendingColor; // I don't think we want colored text in the text box lol -- Feldy
			}
			else
			{
				nameplateColorPicker.BackPanelColor = Rocket.CustomizationDummy.Nameplate.TextColor;
				//nameplateTextBox.TextColor = Rocket.CustomizationDummy.Nameplate.TextColor;
			}
		}

		private void UpdateNameplateAlignButtons()
		{
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
		}

		private void UpdateHSLMenuVisibility()
		{
			if (GetFocusedColorPicker(out _))
			{
				hslMenu.UpdateKeyboardCapture();

				if (customizationPanelBackground.HasChild(customizationControlPanel))
					customizationPanelBackground.ReplaceChildWith(customizationControlPanel, hslMenu);
			}
			else
			{
				if (customizationPanelBackground.HasChild(hslMenu))
					customizationPanelBackground.ReplaceChildWith(hslMenu, customizationControlPanel);
			}
		}

		private void UpdateKeyboardCapture()
		{
			Main.blockInput = !Main.keyState.KeyPressed(Keys.Escape) && !Main.keyState.KeyPressed(Keys.R);

			if (Main.keyState.KeyPressed(Keys.Left))
			{
				leftButton.TriggerRemoteInteraction();
				PickPreviousModule();
			}
			else if(Main.keyState.KeyPressed(Keys.Right))
			{
				rightButton.TriggerRemoteInteraction();
				PickNextModule();
			}

			if (GetFocusedColorPicker(out _))
 				hslMenu.UpdateKeyboardCapture();
		}
		#endregion

		#region Control actions

		private void PickPreviousModule()
		{
			rocketPreview.PreviousModule();
			RefreshPatternColorPickers();
			AllLoseFocus();
		}

		private void PickNextModule()
		{
			rocketPreview.NextModule();
			RefreshPatternColorPickers();
			AllLoseFocus();
		}

		private void OnCurrentModuleChange(string moduleName, int moduleIndex)
		{
			currentModuleName = moduleName;
			customizationPanelBackground.ReplaceChildWith(patternConfigPanel, CreatePatternConfigPanel());
			RefreshPatternColorPickers();
		}

		private void ApplyCustomizationChanges()
		{
			AllLoseFocus();

			Rocket.ApplyCustomizationChanges();

			RefreshPatternColorPickers();
		}

		private void CancelCustomizationChanges()
		{
			AllLoseFocus();

			Rocket.RefreshCustomizationDummy();

			RefreshPatternColorPickers();
		}

		private void ResetRocketToDefaults()
		{
			AllLoseFocus();

			Rocket.ResetCustomizationDummyToDefault();

			RefreshPatternColorPickers();
		}

		private void ResetCurrentModuleToDefaults()
		{
			AllLoseFocus();

			Rocket.ResetDummyModuleToDefault(currentModuleName);

			RefreshPatternColorPickers();
		}

		private void OnHSLMenuCancel()
		{
			if (GetFocusedColorPicker(out var item))
			{
				if (item.colorIndex >= 0)
				{
					item.picker.BackPanelColor = hslMenu.PreviousColor;
					currentModule.Pattern.SetColor(item.colorIndex, hslMenu.PreviousColor);
				}
				else if (item.colorIndex == -1)
				{
					Rocket.CustomizationDummy.Nameplate.TextColor = hslMenu.PreviousColor;
				}
			}

			ColorPickersLoseFocus();

		}
		#endregion

		#region Pattern selection methods
		public void SelectPattern(UIPatternIcon icon)
		{
			currentPatternIcon = icon;
			Rocket.CustomizationDummy.Modules[currentModuleName].Pattern = currentPatternIcon.Pattern;
			RefreshPatternColorPickers();
		}

		private void RefreshPatternColorPickers()
		{
			UpdatePatternConfig();

			if (patternColorPickers is not null)
				foreach (var (picker, _) in patternColorPickers)
					picker.Remove();

			colorPickerSeparator?.Remove();
			resetPatternButton?.Remove();

			CreatePatternColorPickers();
		}

		private List<(UIFocusIconButton, int)> CreatePatternColorPickers()
		{
			patternColorPickers = new();

			var indexes = currentPatternIcon.Pattern.GetUserModifiableIndexes();

			float iconSize = 32f + 8f;
			float iconLeftOffset = 3f;

			for (int i = 0; i < indexes.Count; i++)
			{
				UIFocusIconButton colorPicker = new()
				{
					HAlign = 0f,
					Top = new(0f, 0.04f),
					Left = new(iconSize * i + iconLeftOffset, 0f),
					FocusContext = "RocketCustomizationColorPicker"
				};

				colorPicker.OnLeftClick += (_, _) => { colorPicker.HasFocus = true; };
				colorPicker.OnFocusGain = PatternColorPickerOnFocusGain;

				colorPicker.OnRightClick += (_, _) => { colorPicker.HasFocus = false; };
				colorPicker.OnFocusLost += PatternColorPickerOnFocusLost;

				patternConfigPanel.Append(colorPicker);
				patternColorPickers.Add((colorPicker, indexes[i]));
			}

			colorPickerSeparator = new()
			{
				Height = new(32f, 0f),
				Top = new(0f, 0.04f),
				Left = new(iconSize * indexes.Count + iconLeftOffset - 1f, 0f),
				Color = new Color(89, 116, 213, 255) * 0.9f
			};
			patternConfigPanel.Append(colorPickerSeparator);

			resetPatternButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetWhite"))
			{
				HAlign = 0f,
				Top = new(0f, 0.04f),
				Left = new(iconSize * indexes.Count + iconLeftOffset + 7f, 0f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.ResetPattern")
			};
			resetPatternButton.OnLeftClick += (_, _) => ResetCurrentPatternToDefaults();
			patternConfigPanel.Append(resetPatternButton);

			return patternColorPickers;
		}

		private void ResetCurrentPatternToDefaults()
		{
			//UpdatePatternConfig();
			ColorPickersLoseFocus();

			var defaultPattern = CustomizationStorage.GetPattern(currentModuleName, currentModule.Pattern.Name);
			currentModule.Pattern = defaultPattern.Clone();
			currentPatternIcon.Pattern = defaultPattern.Clone();

			if(GetFocusedColorPicker(out var item) && item.colorIndex >= 0)
			{
				item.picker.BackPanelColor = defaultPattern.GetColor(item.colorIndex);
			}

		}
		#endregion

		#region Focus handlers
		private void AllLoseFocus()
		{
			nameplateTextBox.HasFocus = false;
			ColorPickersLoseFocus();
		}

		private bool GetFocusedColorPicker(out (UIFocusIconButton picker, int colorIndex) focused)
		{
			if (patternColorPickers is not null)
			{
				foreach (var (picker, colorIndex) in patternColorPickers)
				{
					if (picker.HasFocus)
					{
						focused = (picker, colorIndex);
						return true;
					}
				}
			}
			
			if (nameplateColorPicker.HasFocus)
			{
				focused = (nameplateColorPicker, -1);
				return true;
			}

			focused = (null, int.MinValue);
			return false;
		}

		private void ColorPickersLoseFocus()
		{
			if (GetFocusedColorPicker(out var item))
				item.picker.HasFocus = false;
		}

		private void NameplateTextOnFocusGain()
		{
			rocketPreview.SetModule("EngineModule");
		}

		private void NameplateTextOnFocusLost()
		{
		}

		private void NameplateColorPickerOnFocusGain()
		{
			rocketPreview.SetModule("EngineModule");

			hslMenu.SetColorHSL(Rocket.CustomizationDummy.Nameplate.TextColor.ToHSL());
			hslMenu.CaptureCurrentColor();
		}

		private void NameplateColorPickerOnFocusLost()
		{
		}

		private void PatternColorPickerOnFocusGain()
		{
			if(GetFocusedColorPicker(out var item) && item.colorIndex >= 0)
			{
				hslMenu.SetColorHSL(currentModule.Pattern.GetColor(item.colorIndex).ToHSL());
				hslMenu.CaptureCurrentColor();
			}
		}

		private void PatternColorPickerOnFocusLost()
		{
		}
		#endregion

		#region UI creation methods
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

			leftButton = new(ModContent.Request<Texture2D>(buttonsPath + "BackArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "BackArrowBorder", mode))
			{
				VAlign = 0.5f,
				Left = new StyleDimension(0f, 0f),
			};
			leftButton.SetVisibility(1f, 1f, 1f);
			leftButton.CheckInteractible = () => !rocketPreview.AnimationActive;
			leftButton.OnLeftClick += (_, _) => PickPreviousModule();
			
			modulePicker.Append(leftButton);

			rightButton = new(ModContent.Request<Texture2D>(buttonsPath + "ForwardArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "ForwardArrowBorder", mode))
			{
				VAlign = 0.5f,
				Left = new StyleDimension(0, 0.79f),
			};
			rightButton.SetVisibility(1f, 1f, 1f);
			rightButton.CheckInteractible = () => !rocketPreview.AnimationActive;
			rightButton.OnLeftClick += (_, _) => PickNextModule();
	
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

			resetModuleButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetWhite"))
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
				FocusContext = "TextBox",
				OnFocusGain = NameplateTextOnFocusGain,
				OnFocusLost = NameplateTextOnFocusLost
			};
			nameplateConfigPanel.Append(nameplateTextBox);

			nameplateColorPicker = new()
			{
				VAlign = 0.5f,
				HAlign = 0f,
				Left = new(0f, 0.482f),
				FocusContext = "RocketCustomizationColorPicker"
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
					rocketPreview.SetModule("EngineModule");
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
					rocketPreview.SetModule("EngineModule");
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
					rocketPreview.SetModule("EngineModule");
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
					rocketPreview.SetModule("EngineModule");
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
					rocketPreview.SetModule("EngineModule");
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
					rocketPreview.SetModule("EngineModule");
					Rocket.CustomizationDummy.Nameplate.VerticalAlignment = TextAlignmentVertical.Bottom;
				}
			};
			alignBottom.OnLeftClick += (_, _) => alignBottom.HasFocus = true;
			nameplateConfigPanel.Append(alignBottom);

			return nameplateConfigPanel;
		}

		private UIPanel CreateDetailConfigPanel()
		{
			detailConfigPanel = new()
			{
				Width = new(0, 0.99f),
				Height = new(0, 0.22f),
				HAlign = 0.5f,
				Top = new(0f, 0.36f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			detailConfigPanel.SetPadding(0f);
			return detailConfigPanel;
		}

		private UIPanel CreatePatternConfigPanel()
		{
			patternConfigPanel = new()
			{
				Width = new(0, 0.99f),
				Height = new(0, 0.4f),
				HAlign = 0.5f,
				Top = new(0f, 0.595f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
			};
			patternConfigPanel.SetPadding(6f);
			patternConfigPanel.PaddingTop = 0f;

			patternSelector = CustomizationStorage.ProvidePatternUI(currentModuleName);
			var icons = patternSelector.OfType<UIPatternIcon>().ToList();

			icons.ForEach((icon) => icon.OnLeftClick += (_, icon) => SelectPattern(icon as UIPatternIcon));

			patternConfigPanel.Append(patternSelector);

			return patternConfigPanel;
		}
		#endregion 
	}
}
