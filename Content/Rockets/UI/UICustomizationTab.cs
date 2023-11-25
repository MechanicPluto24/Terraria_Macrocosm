using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICustomizationTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        private Rocket rocket = new();
        public Rocket Rocket
        {
            get => rocket;
            set
            {
                bool changed = rocket != value;
                rocket = value;

                if (changed)
                    OnRocketChanged();
            }
        }


        private readonly Dictionary<Rocket, Rocket> rocketDummyPairs = new();
        public Rocket CustomizationDummy
        {
            get
            {
                if (!rocketDummyPairs.ContainsKey(Rocket))
                    rocketDummyPairs[Rocket] = Rocket.VisualClone();

                return rocketDummyPairs[Rocket];
            }
            set
            {
                rocketDummyPairs[Rocket] = value;
            }
        }

        private readonly Dictionary<(Rocket rocket, string moduleName), List<UIPatternIcon>> dummyPatternEdits = new();
        private UIPatternIcon GetCurrentDummyEdit(string name)
             => dummyPatternEdits[(Rocket, currentModule.Name)].FirstOrDefault(icon => icon.Pattern.Name == name);

        private UIPanel rocketPreviewBackground;
        private UIHoverImageButton rocketPreviewZoomButton;
        private UIRocketPreviewLarge rocketPreview;

        private UIPanel customizationPanelBackground;

        private UIPanel modulePicker;
        private UIText modulePickerTitle;
        private UIHoverImageButton leftButton;
        private UIHoverImageButton rightButton;
        private RocketModule currentModule;
        private RocketModule lastModule;

        private UIPanel rocketCustomizationControlPanel;
        private UIPanelIconButton rocketApplyButton;
        private UIPanelIconButton rocketCancelButton;
        private UIPanelIconButton rocketResetButton;
        private UIPanelIconButton rocketCopyButton;
        private UIPanelIconButton rocketPasteButton;

        private UIPanel nameplateConfigPanel;
        private UIInputTextBox nameplateTextBox;
        private UIPanelIconButton nameplateColorPicker;
        private UIPanelIconButton alignLeft;
        private UIPanelIconButton alignCenterHorizontal;
        private UIPanelIconButton alignRight;
        private UIPanelIconButton alignTop;
        private UIPanelIconButton alignCenterVertical;
        private UIPanelIconButton alignBottom;

        private UIPanel detailConfigPanel;

        private UIPanel patternConfigPanel;
        private UIListScrollablePanel patternSelector;
        private UIPatternIcon currentPatternIcon;

        private UIPanelIconButton resetPatternButton;
        private UIVerticalSeparator colorPickerSeparator;
        private List<(UIPanelIconButton picker, int colorIndex)> patternColorPickers;

        private UIColorMenuHSL hslMenu;
        private float luminanceSliderFactor = 0.85f;

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

            BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
            BorderColor = UITheme.Current.TabStyle.BorderColor;

            rocketPreviewBackground = CreateRocketPreview();
            Append(rocketPreviewBackground);

            customizationPanelBackground = new()
            {
                Width = new(0, 0.6f),
                Height = new(0, 1f),
                HAlign = 0f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
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

            rocketCustomizationControlPanel = CreateRocketControlPanel();
            customizationPanelBackground.Append(rocketCustomizationControlPanel);

            hslMenu = new(luminanceSliderFactor)
            {
                HAlign = 0.98f,
                Top = new(0f, 0.092f)
            };
            hslMenu.SetupApplyAndCancelButtons(ColorPickersLoseFocus, OnHSLMenuCancel);

            customizationPanelBackground.Activate();
        }

        public void OnTabOpen()
        {
            RefreshPatternColorPickers();
        }

        public void OnTabClose()
        {
            Main.blockInput = false;
            AllLoseFocus();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CustomizationDummy.ForcedStationaryAppearance = true;
            rocketPreview.RocketDummy = CustomizationDummy;
            currentModule ??= CustomizationDummy.Modules["CommandPod"];
            lastModule ??= currentModule;

            UpdatePatternConfig();
            UpdatePatternColorPickers();

            UpdateNamplateTextBox();
            UpdateNameplateColorPicker();
            UpdateNameplateAlignButtons();

            UpdateHSLMenuVisibility();
            UpdateModulePicker();
            UpdateKeyboardCapture();
        }

        private void OnRocketChanged()
        {
            RefreshPatternConfigPanel();
        }

        #region Update methods
        private void UpdateCurrentModule()
        {

        }

        private void UpdatePatternConfig()
        {
            Pattern currentDummyPattern = CustomizationDummy.Modules[currentModule.Name].Pattern;

            if (patternSelector.OfType<UIPatternIcon>().Any())
            {
                currentPatternIcon = patternSelector.OfType<UIPatternIcon>().FirstOrDefault(icon => icon.Pattern.Name == currentDummyPattern.Name);
                currentPatternIcon.Pattern = currentDummyPattern;
                currentPatternIcon.HasFocus = true;

                if (patternColorPickers is null)
                    CreatePatternColorPickers();
            }
        }

        private void UpdatePatternColorPickers()
        {
            foreach (var (picker, colorIndex) in patternColorPickers)
            {
                if (picker.HasFocus)
                {
                    if (rocketPreview.ZoomedOut)
                    {
                        foreach (var module in CustomizationDummy.Modules)
                        {
                            var modulePattern = CustomizationDummy.Modules[module.Key].Pattern;

                            //TODO: check per color indexes instead
                            if (modulePattern.Name == currentPatternIcon.Pattern.Name && hslMenu.PendingChange)
                                CustomizationDummy.Modules[module.Key].Pattern = modulePattern.WithColor(colorIndex, hslMenu.PendingColor);
                        }
                    }
                    else
                    {
                        if (hslMenu.PendingChange)
                        {
                            currentModule.Pattern = currentModule.Pattern.WithColor(colorIndex, hslMenu.PendingColor);
                            currentPatternIcon.Pattern = currentModule.Pattern.WithColor(colorIndex, hslMenu.PendingColor);
                        }
                    }
                }

                picker.BackPanelColor = currentModule.Pattern.GetColor(colorIndex);
            }
        }

        private void UpdateNamplateTextBox()
        {
            if (nameplateTextBox.HasFocus)
                CustomizationDummy.Nameplate.Text = nameplateTextBox.Text;
            else
                nameplateTextBox.Text = CustomizationDummy.Nameplate.Text;
        }

        private void UpdateNameplateColorPicker()
        {
            if (nameplateColorPicker.HasFocus)
            {
                CustomizationDummy.Nameplate.TextColor = hslMenu.PendingColor;
                nameplateColorPicker.BackPanelColor = hslMenu.PendingColor;
            }
            else
            {
                nameplateColorPicker.BackPanelColor = CustomizationDummy.Nameplate.TextColor;
            }
        }

        private void UpdateNameplateAlignButtons()
        {
            switch (CustomizationDummy.Nameplate.HAlign)
            {
                case TextHorizontalAlign.Left: alignLeft.HasFocus = true; break;
                case TextHorizontalAlign.Right: alignRight.HasFocus = true; break;
                case TextHorizontalAlign.Center: alignCenterHorizontal.HasFocus = true; break;
            }

            switch (CustomizationDummy.Nameplate.VAlign)
            {
                case TextVerticalAlign.Top: alignTop.HasFocus = true; break;
                case TextVerticalAlign.Bottom: alignBottom.HasFocus = true; break;
                case TextVerticalAlign.Center: alignCenterVertical.HasFocus = true; break;
            }
        }

        private void UpdateHSLMenuVisibility()
        {
            if (GetFocusedColorPicker(out _))
            {
                hslMenu.UpdateKeyboardCapture();

                if (customizationPanelBackground.HasChild(rocketCustomizationControlPanel))
                    customizationPanelBackground.ReplaceChildWith(rocketCustomizationControlPanel, hslMenu);
            }
            else
            {
                if (customizationPanelBackground.HasChild(hslMenu))
                    customizationPanelBackground.ReplaceChildWith(hslMenu, rocketCustomizationControlPanel);
            }
        }

        private void UpdateModulePicker()
        {
        }

        private void UpdateKeyboardCapture()
        {
            Main.blockInput = !Main.keyState.KeyPressed(Keys.Escape) && !Main.keyState.KeyPressed(Keys.R);

            bool colorPickerSelected = GetFocusedColorPicker(out var colorPicker);
            int indexInList = patternColorPickers.IndexOf(colorPicker);

            if (Main.keyState.KeyPressed(Keys.Left))
            {
                if (colorPicker.colorIndex >= 0)
                {
                    if (indexInList - 1 >= 0)
                        patternColorPickers[indexInList - 1].picker.HasFocus = true;
                }
                else
                {
                    leftButton.TriggerRemoteInteraction();
                    PickPreviousModule();
                }
            }
            else if (Main.keyState.KeyPressed(Keys.Right))
            {
                if (colorPicker.colorIndex >= 0)
                {
                    if (indexInList + 1 < patternColorPickers.Count)
                        patternColorPickers[indexInList + 1].picker.HasFocus = true;
                }
                else
                {
                    rightButton.TriggerRemoteInteraction();
                    PickNextModule();
                }
            }

            if (colorPickerSelected)
                hslMenu.UpdateKeyboardCapture();
        }
        #endregion

        #region Control actions
        private void PickPreviousModule()
        {
            if (rocketPreview.ZoomedOut)
                return;

            rocketPreview.PreviousModule();
            RefreshPatternColorPickers();
            AllLoseFocus();
        }

        private void PickNextModule()
        {
            if (rocketPreview.ZoomedOut)
                return;

            rocketPreview.NextModule();
            RefreshPatternColorPickers();
            AllLoseFocus();
        }

        private void JumpToModule(string moduleName)
        {
            rocketPreview.SetModule(moduleName);
            RefreshPatternColorPickers();
        }

        private void OnCurrentModuleChange(string moduleName, int moduleIndex)
        {
            currentModule = CustomizationDummy.Modules[moduleName];
            modulePickerTitle.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules." + moduleName));
            RefreshPatternConfigPanel();
        }

        private void RefreshPatternConfigPanel()
        {
            customizationPanelBackground.ReplaceChildWith(patternConfigPanel, CreatePatternConfigPanel());
            RefreshPatternColorPickers();
        }

        private void OnPreviewZoomIn()
        {
            rocketPreviewZoomButton.SetImage(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Buttons/ZoomOutButton"));
            rocketPreviewZoomButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Common.ZoomOut");

            currentModule = lastModule;
            modulePickerTitle.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules." + currentModule.Name));
            RefreshPatternConfigPanel();
        }

        private void OnPreviewZoomOut()
        {
            rocketPreviewZoomButton.SetImage(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Buttons/ZoomInButton"));
            rocketPreviewZoomButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Common.ZoomIn");

            hslMenu.CaptureCurrentColor();

            lastModule = currentModule;
            currentModule = CustomizationDummy.Modules["BoosterRight"];
            modulePickerTitle.SetText("");
            RefreshPatternConfigPanel();
        }

        private void ApplyCustomizationChanges()
        {
            AllLoseFocus();

            Rocket.ApplyCustomizationChanges(CustomizationDummy);

            RefreshPatternColorPickers();
        }

        private void DiscardCustomizationChanges()
        {
            AllLoseFocus();

            CustomizationDummy = Rocket.Clone();

            UpdatePatternConfig();
            currentPatternIcon.Pattern = CustomizationDummy.Modules[currentModule.Name].Pattern;

            RefreshPatternColorPickers();
        }

        private void ResetRocketToDefaults()
        {
            AllLoseFocus();
            CustomizationDummy.ResetCustomizationToDefault();
            RefreshPatternColorPickers();
        }

        private void CopyRocketData()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            string json = CustomizationDummy.GetCustomizationDataToJSON();
            Platform.Get<IClipboard>().Value = json;
        }

        private void PasteRocketData()
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            string json = Platform.Get<IClipboard>().Value;

            try
            {
                CustomizationDummy.ApplyRocketCustomizationFromJSON(json);
                RefreshPatternColorPickers();
            }
            catch (Exception ex)
            {
                Utility.Chat(ex.Message);
            }
        }

        private void OnHSLMenuCancel()
        {
            if (GetFocusedColorPicker(out var item))
            {
                if (item.colorIndex >= 0)
                {
                    item.picker.BackPanelColor = hslMenu.PreviousColor;

                    if (rocketPreview.ZoomedOut)
                    {
                        foreach (var module in CustomizationDummy.Modules)
                        {
                            var modulePattern = CustomizationDummy.Modules[module.Key].Pattern;

                            if (modulePattern.Name == currentPatternIcon.Pattern.Name)
                                CustomizationDummy.Modules[module.Key].Pattern = modulePattern.WithColor(item.colorIndex, hslMenu.PendingColor, evenIfNotUserModifiable: true);
                        }
                    }
                    else
                    {
                        currentModule.Pattern = currentModule.Pattern.WithColor(item.colorIndex, hslMenu.PreviousColor);
                    }
                }
                else if (item.colorIndex == -1)
                {
                    CustomizationDummy.Nameplate.TextColor = hslMenu.PreviousColor;
                }
            }
            ColorPickersLoseFocus();
        }
        #endregion

        #region Pattern selection methods
        public void SelectPattern(UIPatternIcon icon)
        {
            currentPatternIcon = icon;

            if (rocketPreview.ZoomedOut)
            {
                foreach (var module in CustomizationDummy.Modules)
                {
                    if (CustomizationStorage.TryGetPattern(module.Key, icon.Pattern.Name, out Pattern defaultPattern))
                    {
                        var colorData = defaultPattern.ColorData[0].WithUserColor(CustomizationDummy.Modules[module.Key].Pattern.ColorData[0].Color);
                        var newPattern = defaultPattern.WithColorData(colorData);
                        CustomizationDummy.Modules[module.Key].Pattern = newPattern;
                    }
                }
            }
            else
            {
                if (CustomizationStorage.TryGetPattern(currentModule.Name, icon.Pattern.Name, out Pattern defaultPattern))
                {
                    var colorData = defaultPattern.ColorData[0].WithUserColor(currentModule.Pattern.ColorData[0].Color);
                    var newPattern = defaultPattern.WithColorData(colorData);
                    currentModule.Pattern = newPattern;
                }
            }

            RefreshPatternColorPickers();
        }

        private void RefreshPatternColorPickers()
        {
            UpdatePatternConfig();
            ClearPatternColorPickers();
            CreatePatternColorPickers();
        }

        private void ClearPatternColorPickers()
        {
            if (patternColorPickers is not null)
                foreach (var (picker, _) in patternColorPickers)
                    picker.Remove();

            colorPickerSeparator?.Remove();
            resetPatternButton?.Remove();
        }

        private List<(UIPanelIconButton, int)> CreatePatternColorPickers()
        {
            patternColorPickers = new();

            var indexes = currentModule.Pattern.UserModifiableIndexes;

            float iconSize = 32f + 8f;
            float iconLeftOffset = 3f;

            for (int i = 0; i < indexes.Count; i++)
            {
                UIPanelIconButton colorPicker = new()
                {
                    HAlign = 0f,
                    Top = new(0f, 0.04f),
                    Left = new(iconSize * i + iconLeftOffset, 0f),
                    FocusContext = "RocketCustomizationColorPicker",
                    OverrideBackgroundColor = true
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
                Color = UITheme.Current.SeparatorColor
            };
            patternConfigPanel.Append(colorPickerSeparator);

            resetPatternButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetWhite"))
            {
                HAlign = 0f,
                Top = new(0f, 0.04f),
                Left = new(iconSize * indexes.Count + iconLeftOffset + 7f, 0f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.ResetPattern")
            };
            resetPatternButton.OnLeftClick += (_, _) => ResetCurrentPatternToDefaults();
            patternConfigPanel.Append(resetPatternButton);

            return patternColorPickers;
        }

        private void ResetCurrentPatternToDefaults()
        {
            //UpdatePatternConfig();
            ColorPickersLoseFocus();

            if (rocketPreview.ZoomedOut)
            {
                foreach (var module in CustomizationDummy.Modules)
                {
                    var currentModulePattern = CustomizationDummy.Modules[module.Key].Pattern;

                    if (currentModulePattern.Name == currentPatternIcon.Pattern.Name)
                    {
                        var defaultPattern = CustomizationStorage.GetPattern(module.Key, currentModulePattern.Name);
                        CustomizationDummy.Modules[module.Key].Pattern = defaultPattern;
                        currentPatternIcon.Pattern = defaultPattern;
                    }
                }
            }
            else
            {
                var defaultPattern = CustomizationStorage.GetPattern(currentModule.Name, currentModule.Pattern.Name);
                currentModule.Pattern = defaultPattern;
                currentPatternIcon.Pattern = defaultPattern;
            }

            RefreshPatternColorPickers();
        }
        #endregion

        #region Focus handlers
        private void AllLoseFocus()
        {
            nameplateTextBox.HasFocus = false;
            ColorPickersLoseFocus();
        }

        private bool GetFocusedColorPicker(out (UIPanelIconButton picker, int colorIndex) focused)
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
            JumpToModule("EngineModule");
        }

        private void NameplateTextOnFocusLost()
        {
        }

        private void NameplateColorPickerOnFocusGain()
        {
            JumpToModule("EngineModule");

            hslMenu.SetColorHSL(CustomizationDummy.Nameplate.TextColor.ToScaledHSL(luminanceSliderFactor));
            hslMenu.CaptureCurrentColor();
        }

        private void NameplateColorPickerOnFocusLost()
        {
        }

        private void PatternColorPickerOnFocusGain()
        {
            if (GetFocusedColorPicker(out var item) && item.colorIndex >= 0)
            {
                hslMenu.SetColorHSL(currentModule.Pattern.GetColor(item.colorIndex).ToScaledHSL(luminanceSliderFactor));
                hslMenu.CaptureCurrentColor();
            }
        }

        private void PatternColorPickerOnFocusLost()
        {
        }
        #endregion

        #region UI creation methods
        private const string buttonsPath = "Macrocosm/Assets/Textures/UI/Buttons/";
        private const string symbolsPath = "Macrocosm/Assets/Textures/UI/Symbols/";


        private UIPanel CreateRocketPreview()
        {
            rocketPreviewBackground = new()
            {
                Width = new(0, 0.4f),
                Height = new(0, 1f),
                Left = new(0, 0.605f),
                HAlign = 0f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            rocketPreviewBackground.SetPadding(2f);
            rocketPreviewBackground.OverflowHidden = true;
            rocketPreviewBackground.Activate();
            Append(rocketPreviewBackground);

            rocketPreview = new()
            {
                OnZoomedIn = OnPreviewZoomIn,
                OnZoomedOut = OnPreviewZoomOut,
                OnModuleChange = OnCurrentModuleChange,
            };
            rocketPreviewBackground.Append(rocketPreview);

            rocketPreviewZoomButton = new(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Buttons/ZoomOutButton"), ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Buttons/ZoomButtonBorder"))
            {
                Left = new(10, 0),
                Top = new(10, 0),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.ZoomOut")
            };
            rocketPreviewZoomButton.OnLeftClick += (_, _) => rocketPreview.ZoomedOut = !rocketPreview.ZoomedOut;
            rocketPreviewBackground.Append(rocketPreviewZoomButton);

            return rocketPreviewBackground;
        }

        private UIPanel CreateModulePicker()
        {
            currentModule ??= CustomizationDummy.Modules["CommandPod"];
            lastModule ??= CustomizationDummy.Modules["CommandPod"];

            var mode = ReLogic.Content.AssetRequestMode.ImmediateLoad;

            modulePicker = new()
            {
                Width = new(0, 0.36f),
                Height = new(0, 0.25f),
                HAlign = 0.007f,
                Top = new(0f, 0.092f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            modulePicker.SetPadding(0f);

            modulePickerTitle = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules.CommandPod"), 0.9f, false)
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
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            modulePicker.Append(moduleIconPreviewPanel);

            leftButton = new(ModContent.Request<Texture2D>(buttonsPath + "BackArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "BackArrowBorder", mode))
            {
                VAlign = 0.5f,
                Left = new StyleDimension(0f, 0f),
                CheckInteractible = () => !rocketPreview.ZoomedOut
            };
            leftButton.OnLeftClick += (_, _) => PickPreviousModule();

            modulePicker.Append(leftButton);

            rightButton = new(ModContent.Request<Texture2D>(buttonsPath + "ForwardArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "ForwardArrowBorder", mode))
            {
                VAlign = 0.5f,
                Left = new StyleDimension(0, 0.79f),
                CheckInteractible = () => !rocketPreview.ZoomedOut
            };
            rightButton.OnLeftClick += (_, _) => PickNextModule();

            modulePicker.Append(rightButton);
            return modulePicker;
        }

        private UIPanel CreateRocketControlPanel()
        {
            rocketCustomizationControlPanel = new()
            {
                Width = new(0f, 0.62f),
                Height = new(0, 0.25f),
                HAlign = 0.98f,
                Top = new(0f, 0.092f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            rocketCustomizationControlPanel.SetPadding(2f);
            customizationPanelBackground.Append(rocketCustomizationControlPanel);

            rocketCopyButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"))
            {
                VAlign = 0.9f,
                Left = new(0f, 0.18f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.CopyRocket")
            };
            rocketCopyButton.OnLeftMouseDown += (_, _) => CopyRocketData();
            rocketCustomizationControlPanel.Append(rocketCopyButton);

            rocketPasteButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"))
            {
                VAlign = 0.9f,
                Left = new(0f, 0.295f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.PasteRocket")
            };
            rocketPasteButton.OnLeftMouseDown += (_, _) => PasteRocketData();
            rocketCustomizationControlPanel.Append(rocketPasteButton);

            UIVerticalSeparator separator1 = new()
            {
                Height = new(32f, 0f),
                VAlign = 0.9f,
                Left = new(0f, 0.425f),
                Color = UITheme.Current.SeparatorColor
            };
            rocketCustomizationControlPanel.Append(separator1);

            rocketResetButton = new(ModContent.Request<Texture2D>(symbolsPath + "ResetWhite"))
            {
                VAlign = 0.9f,
                Left = new(0f, 0.448f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.ResetRocket")
            };
            rocketResetButton.OnLeftClick += (_, _) => ResetRocketToDefaults();
            rocketCustomizationControlPanel.Append(rocketResetButton);

            UIVerticalSeparator separator2 = new()
            {
                Height = new(32f, 0f),
                VAlign = 0.9f,
                Left = new(0f, 0.571f),
                Color = UITheme.Current.SeparatorColor
            };
            rocketCustomizationControlPanel.Append(separator2);

            rocketCancelButton = new(ModContent.Request<Texture2D>(symbolsPath + "CrossmarkRed"))
            {
                VAlign = 0.9f,
                Left = new(0f, 0.6f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.Cancel")
            };
            rocketCancelButton.OnLeftClick += (_, _) => DiscardCustomizationChanges();
            rocketCustomizationControlPanel.Append(rocketCancelButton);

            rocketApplyButton = new(ModContent.Request<Texture2D>(symbolsPath + "CheckmarkGreen"))
            {
                VAlign = 0.9f,
                Left = new(0f, 0.715f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.Apply")
            };
            rocketApplyButton.OnLeftClick += (_, _) => ApplyCustomizationChanges();
            rocketCustomizationControlPanel.Append(rocketApplyButton);

            // TODO...
            /*
			randomizeButton = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"))
			{
				VAlign = 0.93f,
				Left = new(0f, 0.77f),
				HoverText = Language.GetText("Mods.Macrocosm.UI.Common.RandomizeCustomization")
			};

			randomizeButton.OnLeftMouseDown += (_, _) => RandomizeCustomization();
			rocketCustomizationControlPanel.Append(randomizeButton);
			*/

            return rocketCustomizationControlPanel;
        }

        private UIPanel CreateNameplateConfigPanel()
        {
            nameplateConfigPanel = new()
            {
                Width = new StyleDimension(0, 0.99f),
                Height = new StyleDimension(0, 0.08f),
                HAlign = 0.5f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            nameplateConfigPanel.SetPadding(0f);

            nameplateTextBox = new(Language.GetText("Mods.Macrocosm.Common.Rocket").Value)
            {
                Width = new(0f, 0.46f),
                Height = new(0f, 0.74f),
                HAlign = 0.02f,
                VAlign = 0.55f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                HoverBorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor,
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
                    JumpToModule("EngineModule");
                    CustomizationDummy.Nameplate.HAlign = TextHorizontalAlign.Left;
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
                    CustomizationDummy.Nameplate.HAlign = TextHorizontalAlign.Center;
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
                    CustomizationDummy.Nameplate.HAlign = TextHorizontalAlign.Right;
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
                    CustomizationDummy.Nameplate.VAlign = TextVerticalAlign.Top;
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
                    CustomizationDummy.Nameplate.VAlign = TextVerticalAlign.Center;
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
                    CustomizationDummy.Nameplate.VAlign = TextVerticalAlign.Bottom;
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
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
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
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            patternConfigPanel.SetPadding(6f);
            patternConfigPanel.PaddingTop = 0f;

            foreach (var module in Rocket.Modules.Keys)
            {
                if (module == currentModule.Name)
                {
                    patternSelector = CustomizationStorage.ProvidePatternUI(module);
                    foreach (var icon in patternSelector.OfType<UIPatternIcon>().ToList())
                        icon.OnLeftClick += (_, icon) => SelectPattern(icon as UIPatternIcon);
                }
            }

            patternConfigPanel.Append(patternSelector);

            return patternConfigPanel;
        }
        #endregion
    }
}
