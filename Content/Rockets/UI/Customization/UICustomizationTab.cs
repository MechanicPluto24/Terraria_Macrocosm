using Macrocosm.Common.Customization;
using Macrocosm.Common.Enums;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Consumables.Unlockables;
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

namespace Macrocosm.Content.Rockets.UI.Customization
{
    public class UICustomizationTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        private record ColorPicker
        {
            public enum ContextType
            {
                Pattern,
                Nameplate
            }

            public ContextType Context { get; }
            public UIPanelIconButton Panel { get; }
            public bool HasFocus
            {
                get => Panel.HasFocus;
                set => Panel.HasFocus = value;
            }

            public Color BackPanelColor
            {
                get => Panel.BackPanelColor;
                set => Panel.BackPanelColor = value;
            }

            public ColorPicker(ContextType context, UIPanelIconButton panel)
            {
                this.Context = context;
                this.Panel = panel;
            }
        }


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

        private UIPanel rocketPreviewBackground;
        private UIHoverImageButton rocketPreviewZoomButton;
        private UIRocketPreviewLarge rocketPreview;

        private UIPanel customizationPanelBackground;

        private UIPanel modulePicker;
        private UIText modulePickerTitle;
        private UIModuleIcon modulePickerIconPanel;
        private UIHoverImageButton leftButton;
        private UIHoverImageButton rightButton;

        private string currentModuleName = "EngineModule";
        private string lastModuleName = "EngineModule";
        private RocketModule CurrentModule => Rocket.ModuleTemplates.FirstOrDefault((module) => module.Name == currentModuleName);

        private UIPanel rocketCustomizationControlPanel;
        private UIPanelIconButton rocketApplyButton;
        private UIPanelIconButton rocketCancelButton;
        private UIPanelIconButton rocketResetButton;
        private UIPanelIconButton rocketCopyButton;
        private UIPanelIconButton rocketPasteButton;

        private Item ItemInUnlockableSlot => Rocket.Inventory[Rocket.SpecialInventorySlot_CustomizationUnlock];
        private UIInventorySlot unlockableItemSlot;
        private UIPanelIconButton unlockableApplyButton;

        private UIPanel nameplateConfigPanel;
        private UIInputTextBox nameplateTextBox;
        private UIPanelIconButton nameplateAcceptResetButton;
        private UIPanelIconButton nameplateColorPicker;
        private UIPanelIconButton alignLeft;
        private UIPanelIconButton alignCenterHorizontal;
        private UIPanelIconButton alignRight;
        private UIPanelIconButton alignTop;
        private UIPanelIconButton alignCenterVertical;
        private UIPanelIconButton alignBottom;

        private UIListScrollablePanel decalConfigPanel;
        private UIDecalIcon currentDecalIcon;

        private UIPanel patternConfigPanel;
        private UIListScrollablePanel patternSelector;
        private UIPatternIcon currentPatternIcon;

        private UIPanelIconButton resetPatternButton;
        private UIVerticalSeparator colorPickerSeparator;

        private Dictionary<Color, ColorPicker> patternColorPickers;
        private ColorPicker nameplatePicker;

        private UIColorMenuHSL hslMenu;
        private const float luminanceSliderFactor = 0.85f;

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
                Width = new(0, 0.596f),
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

            decalConfigPanel = CreateDecalConfigPanel();
            customizationPanelBackground.Append(decalConfigPanel);

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
            JumpToModule(currentModuleName);
        }

        private void OnRocketChanged()
        {
            RefreshPatternConfigPanel();
            RefreshDecalConfigPanel();
            rocketCustomizationControlPanel.ReplaceChildWith(unlockableItemSlot, unlockableItemSlot = CreateUnlockableItemSlot());
        }

        public void OnTabOpen()
        {
            Main.stackSplit = 600;
            RefreshPatternColorPickers();
            rocketCustomizationControlPanel.ReplaceChildWith(unlockableItemSlot, unlockableItemSlot = CreateUnlockableItemSlot());
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

            UpdateDecalConfig();
            UpdatePatternConfig();
            UpdatePatternColorPickers();

            UpdateNamplateTextBox();
            UpdateNameplateColorPicker();
            UpdateNameplateAlignButtons();

            UpdateHSLMenuVisibility();
            UpdateModulePicker();
            UpdateKeyboardCapture();
        }

        #region Update methods
        private void UpdateCurrentModule()
        {
        }

        private void UpdateDecalConfig()
        {
            if (CurrentModule is null || !CurrentModule.HasDecal)
                return;

            Decal currentDummyDecal = CurrentModule.Decal;

            var list = decalConfigPanel.OfType<UIDecalIcon>();
            if (decalConfigPanel.OfType<UIDecalIcon>().Any())
            {
                currentDecalIcon = decalConfigPanel.OfType<UIDecalIcon>().FirstOrDefault(icon => icon.Decal.Name == currentDummyDecal.Name);
                currentDecalIcon.Decal = currentDummyDecal;
                currentDecalIcon.HasFocus = true;
            }
        }

        private void UpdatePatternConfig()
        {
            if (!CurrentModule.HasPattern)
                return;

            Pattern currentDummyPattern = CurrentModule.Pattern;
            var list = patternSelector.OfType<UIPatternIcon>();
            if (patternSelector.OfType<UIPatternIcon>().Any())
            {
                UIPatternIcon matchingIcon = patternSelector.OfType<UIPatternIcon>().FirstOrDefault(icon => icon.Pattern.Name == currentDummyPattern.Name);
                if (matchingIcon != null)
                {
                    currentPatternIcon = matchingIcon;
                    currentPatternIcon.Pattern = currentDummyPattern;
                    currentPatternIcon.HasFocus = true;
                }

                patternColorPickers ??= CreatePatternColorPickers();
            }
        }

        private void UpdatePatternColorPickers()
        {
            foreach (var (key, picker) in patternColorPickers)
            {
                if (picker.HasFocus && hslMenu.PendingChange)
                {
                    var modules = rocketPreview.ZoomedOut ? CustomizationDummy.Modules : [CurrentModule];
                    foreach (var module in modules)
                    {
                        var modulePattern = module.Pattern;
                        if (modulePattern.Name == currentPatternIcon.Pattern.Name)
                        {
                            picker.BackPanelColor = hslMenu.PendingColor;
                            module.Pattern.SetColor(key, hslMenu.PendingColor);
                        }
                    }
                }
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
            if (!AlignButtonInteractible())
                return;

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
            Main.blockInput = !Main.keyState.KeyPressed(Keys.Escape);

            bool colorPickerSelected = GetFocusedColorPicker(out var focusedPicker);
            int pickerIndex = patternColorPickers.Values.ToList().IndexOf(focusedPicker);

            if (colorPickerSelected)
            {
                hslMenu.UpdateKeyboardCapture();

                if (Main.keyState.KeyPressed(Keys.Left))
                {
                    if (pickerIndex > 0)
                    {
                        var previousPicker = patternColorPickers.Values.ElementAt(pickerIndex - 1);
                        focusedPicker.HasFocus = false;
                        previousPicker.HasFocus = true;
                    }
                    else
                    {
                        leftButton.TriggerRemoteInteraction();
                        PickPreviousModule();
                    }
                }
                else if (Main.keyState.KeyPressed(Keys.Right))
                {
                    if (pickerIndex < patternColorPickers.Count - 1)
                    {
                        var nextPicker = patternColorPickers.Values.ElementAt(pickerIndex + 1);
                        focusedPicker.HasFocus = false;
                        nextPicker.HasFocus = true;
                    }
                    else
                    {
                        rightButton.TriggerRemoteInteraction();
                        PickNextModule();
                    }
                }
            }
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
            OnCurrentModuleChange(moduleName, -1);
            RefreshPatternColorPickers();
        }

        private void OnCurrentModuleChange(string moduleName, int moduleIndex)
        {
            currentModuleName = moduleName;
            modulePickerTitle.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules." + moduleName + ".DisplayName")); // TODO: use CurrentModule.DisplayName
            modulePickerIconPanel.SetModule(moduleName);
            RefreshPatternConfigPanel();
            RefreshDecalConfigPanel();
        }

        private void RefreshDecalConfigPanel()
        {
            customizationPanelBackground.ReplaceChildWith(decalConfigPanel, CreateDecalConfigPanel());
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

            currentModuleName = lastModuleName;
            modulePickerTitle.SetText(CurrentModule.DisplayName);
            modulePickerIconPanel.SetModule(CurrentModule.Name);
            RefreshPatternConfigPanel();
            RefreshDecalConfigPanel();
        }

        private void OnPreviewZoomOut()
        {
            hslMenu.CaptureCurrentColor();
            rocketPreviewZoomButton.SetImage(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Buttons/ZoomInButton"));
            rocketPreviewZoomButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Common.ZoomIn");

            hslMenu.CaptureCurrentColor();

            lastModuleName = currentModuleName;

            // This a hacky way to see all available color slots for the entire rocket. Still, it can lead to weird behavior
            //TODO: Add a way to fetch common patterns for all modules and never reference a specific module if zoomed out
            currentModuleName = "BoosterLeft";

            modulePickerTitle.SetText("");
            modulePickerIconPanel.ClearModule();
            RefreshPatternConfigPanel();
            RefreshDecalConfigPanel();
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

            UpdateDecalConfig();
            UpdatePatternConfig();
            RefreshPatternColorPickers();
        }

        private void UpdateIconOnDiscard()
        {
        }

        private void ResetRocketToDefaults()
        {
            AllLoseFocus();

            CustomizationDummy.ResetCustomizationToDefault();

            RefreshPatternColorPickers();
        }

        private void ApplyUnlockableItem()
        {
            if (Rocket.CheckUnlockableItemUnlocked(ItemInUnlockableSlot))
                return;

            if (ItemInUnlockableSlot.ModItem is PatternDesign patternDesign)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                PatternManager.SetUnlocked(patternDesign.PatternName, true);
                ItemInUnlockableSlot.TurnToAir();
                RefreshPatternConfigPanel();
            }
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
            if (GetFocusedColorPicker(out var picker))
            {
                if (picker.Context == ColorPicker.ContextType.Pattern)
                {
                    var key = patternColorPickers.FirstOrDefault(p => p.Value == picker).Key;
                    picker.BackPanelColor = hslMenu.PreviousColor;
                    CurrentModule.Pattern.SetColor(key, hslMenu.PreviousColor);
                }
                else if (picker.Context == ColorPicker.ContextType.Nameplate)
                {
                    CustomizationDummy.Nameplate.TextColor = hslMenu.PreviousColor;
                    picker.BackPanelColor = hslMenu.PreviousColor;
                }
            }

            ColorPickersLoseFocus();
        }

        #endregion

        public void SelectDecal(UIDecalIcon icon)
        {
            currentDecalIcon = icon;

            if (rocketPreview.ZoomedOut)
            {
                // Apply common decals
            }
            else
            {
                if (DecalManager.TryGetDecal(icon.Decal.Context, icon.Decal.Name, out Decal decal))
                    CurrentModule.Decal = decal;
            }
        }

        #region Pattern selection methods
        public void SelectPattern(UIPatternIcon icon)
        {
            currentPatternIcon = icon;

            var modules = rocketPreview.ZoomedOut ? CustomizationDummy.Modules : [CurrentModule];
            foreach (var module in modules)
            {
                if (PatternManager.TryGet(icon.Pattern.Name, module.Name, out Pattern defaultPattern))
                {
                    module.Pattern = defaultPattern;
                    currentPatternIcon.Pattern = defaultPattern.Clone();
                }
            }
        }

        private void RefreshPatternColorPickers()
        {
            UpdateCurrentModule();
            UpdatePatternConfig();

            ClearPatternColorPickers();
            CreatePatternColorPickers();
        }

        private void ClearPatternColorPickers()
        {
            if (patternColorPickers is not null)
            {
                foreach (var (_, picker) in patternColorPickers)
                    picker.Panel.Remove();

                patternColorPickers.Clear();
            }

            colorPickerSeparator?.Remove();
            resetPatternButton?.Remove();
        }

        private Dictionary<Color, ColorPicker> CreatePatternColorPickers()
        {
            patternColorPickers?.Clear();

            int index = 0;
            float iconSize = 32f + 8f;
            float iconLeftOffset = 10f;

            if (CurrentModule != null)
            {
                Color[] prioritizedColors = [new(242, 119, 119)];
                patternColorPickers = CurrentModule.Pattern.ColorData
                    .Where(kvp => kvp.Value.IsUserModifiable)
                    .OrderBy(kvp => (
                        !prioritizedColors.Contains(kvp.Key),
                        kvp.Key.GetHue(),
                        kvp.Key.GetSaturation(),
                        kvp.Key.GetBrightness()
                    )).ToDictionary
                    (
                    kvp => kvp.Key,
                    kvp =>
                    {
                        var panel = new UIPanelIconButton
                        {
                            HAlign = 0f,
                            Top = new(0f, 0.04f),
                            Left = new(iconSize * index + iconLeftOffset, 0f),
                            FocusContext = "RocketCustomizationColorPicker",
                            OverrideBackgroundColor = true,
                            BackPanelColor = CurrentModule.Pattern.GetColor(kvp.Key)
                        };
                        index++;
                        var key = kvp.Key;
                        panel.OnLeftClick += (_, _) => { panel.HasFocus = true; };
                        panel.OnFocusGain = () =>
                        {
                            hslMenu.SetColorHSL(CurrentModule.Pattern.GetColor(key).ToScaledHSL(luminanceSliderFactor));
                            hslMenu.CaptureCurrentColor();
                        };

                        panel.OnRightClick += (_, _) => { panel.HasFocus = false; };
                        panel.OnFocusLost = () =>
                        {
                        };

                        patternConfigPanel.Append(panel);
                        return new ColorPicker(
                            context: 0,
                            panel: panel
                        );
                    }
                );
            }

            colorPickerSeparator = new()
            {
                Height = new(32f, 0f),
                Top = new(0f, 0.04f),
                Left = new(iconSize * index + iconLeftOffset - 1f, 0f),
                Color = UITheme.Current.SeparatorColor
            };
            patternConfigPanel.Append(colorPickerSeparator);

            resetPatternButton = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite"))
            {
                HAlign = 0f,
                Top = new(0f, 0.04f),
                Left = new(iconSize * index + iconLeftOffset + 7f, 0f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.ResetPattern")
            };
            resetPatternButton.OnLeftClick += (_, _) => ResetCurrentPatternToDefaults();
            patternConfigPanel.Append(resetPatternButton);

            return patternColorPickers;
        }

        private void ResetCurrentPatternToDefaults()
        {
            ColorPickersLoseFocus();

            var modules = rocketPreview.ZoomedOut ? CustomizationDummy.AvailableModules : [CurrentModule];
            foreach (var module in modules)
            {
                var currentPattern = module.Pattern;
                if (currentPattern.Name == currentPatternIcon.Pattern.Name)
                {
                    var defaultPattern = PatternManager.Get(currentPattern.Name, module.Name);
                    module.Pattern = defaultPattern;
                    SelectPattern(currentPatternIcon);
                }
            }

            RefreshPatternColorPickers();
        }
        #endregion

        #region Focus handlers
        private void AllLoseFocus()
        {
            nameplateTextBox.HasFocus = false;
            nameplateColorPicker.HasFocus = false;
            ColorPickersLoseFocus();
        }

        private bool GetFocusedColorPicker(out ColorPicker focused)
        {
            if (patternColorPickers is not null)
            {
                foreach (var (key, picker) in patternColorPickers)
                {
                    if (picker.HasFocus)
                    {
                        focused = picker;
                        return true;
                    }
                }
            }

            if (nameplateColorPicker.HasFocus)
            {
                focused = nameplatePicker;
                return true;
            }

            focused = null;
            return false;
        }

        /// <summary> Also closes the HSL menu in next update tick </summary>
        private void ColorPickersLoseFocus()
        {
            if (GetFocusedColorPicker(out var picker))
                picker.HasFocus = false;
        }

        private void NameplateTextOnFocusGain()
        {
            JumpToModule("EngineModule");

            nameplateAcceptResetButton.SetPanelTextures(
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkWhite"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkBorderHighlight"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkBorderHighlight")
            );
        }

        private void NameplateTextOnFocusLost()
        {
            nameplateAcceptResetButton.SetPanelTextures(
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight")
            );
        }

        private void NameplateAcceptResetButtonOnClick()
        {
            if (nameplateTextBox.HasFocus)
            {
                nameplateTextBox.HasFocus = false;
            }
            else
            {
                CustomizationDummy.Nameplate.Text = "";
                nameplateTextBox.Text = Language.GetTextValue("Mods.Macrocosm.Common.Rocket");
            }
        }

        #endregion

        #region UI creation methods
        private UIPanel CreateRocketPreview()
        {
            rocketPreviewBackground = new()
            {
                Width = new(0, 0.4f),
                Height = new(0, 1f),
                Left = new(0, 0.6f),
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

            modulePickerTitle = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Modules.CommandPod.DisplayName"), 0.9f, false)
            {
                IsWrapped = false,
                HAlign = 0.5f,
                VAlign = 0.05f,
                TextColor = Color.White
            };
            modulePicker.Append(modulePickerTitle);

            modulePickerIconPanel = new()
            {
                Width = new(0f, 0.6f),
                Height = new(0f, 0.7f),
                HAlign = 0.5f,
                VAlign = 0.6f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            modulePicker.Append(modulePickerIconPanel);

            leftButton = new(ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", mode), ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", mode))
            {
                VAlign = 0.5f,
                Left = new StyleDimension(0f, 0f),
                SpriteEffects = SpriteEffects.FlipHorizontally,
                CheckInteractible = () => !rocketPreview.ZoomedOut
            };
            leftButton.OnLeftClick += (_, _) => PickPreviousModule();

            modulePicker.Append(leftButton);

            rightButton = new(ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", mode), ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", mode))
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

            unlockableItemSlot = CreateUnlockableItemSlot();
            rocketCustomizationControlPanel.Append(unlockableItemSlot);

            unlockableApplyButton = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "Hammer"))
            {
                VAlign = 0.5f,
                Left = new(0f, 0.6f),
                GrayscaleIconIfNotInteractible = true,
                CheckInteractible = () =>
                {
                    return !Rocket.CheckUnlockableItemUnlocked(ItemInUnlockableSlot)
                        && ItemInUnlockableSlot.ModItem is PatternDesign patternDesign;
                }
            };
            unlockableApplyButton.OnLeftClick += (_, _) => ApplyUnlockableItem();
            rocketCustomizationControlPanel.Append(unlockableApplyButton);

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

            rocketResetButton = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite"))
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

            rocketCancelButton = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CrossmarkRed"))
            {
                VAlign = 0.9f,
                Left = new(0f, 0.6f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Customization.Cancel")
            };
            rocketCancelButton.OnLeftClick += (_, _) => DiscardCustomizationChanges();
            rocketCustomizationControlPanel.Append(rocketCancelButton);

            rocketApplyButton = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkGreen"))
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

        private UIInventorySlot CreateUnlockableItemSlot()
        {
            unlockableItemSlot = Rocket.Inventory.ProvideItemSlot(Rocket.SpecialInventorySlot_CustomizationUnlock);
            unlockableItemSlot.HAlign = 0.5f;
            unlockableItemSlot.VAlign = 0.5f;
            return unlockableItemSlot;
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
                TextMaxLength = Nameplate.MaxChars,
                TextScale = 1f,
                AllowSnippets = false,
                FormatText = Nameplate.FormatText,
                FocusContext = "TextBox",
                OnFocusGain = NameplateTextOnFocusGain,
                OnFocusLost = NameplateTextOnFocusLost
            };
            nameplateConfigPanel.Append(nameplateTextBox);

            nameplateAcceptResetButton = new
            (
                Macrocosm.EmptyTex,
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight")
            )
            {
                Width = new(20, 0),
                Height = new(20, 0),
                VAlign = 0.5f,
                Left = new(0, 0.42f),
                BackPanelColor = Color.White,
                FocusedBackPanelColor = Color.White,
                BackPanelBorderColor = Color.Transparent,
                BackPanelHoverBorderColor = Color.White
            };
            nameplateAcceptResetButton.OnLeftClick += (_, _) => NameplateAcceptResetButtonOnClick();
            nameplateConfigPanel.Append(nameplateAcceptResetButton);

            nameplateColorPicker = new()
            {
                VAlign = 0.5f,
                HAlign = 0f,
                Left = new(0f, 0.482f),
                OverrideBackgroundColor = true,
                CheckInteractible = AlignButtonInteractible,
                FocusContext = "RocketCustomizationColorPicker"
            };

            nameplateColorPicker.OnLeftClick += (_, _) => { nameplateColorPicker.HasFocus = true; };
            nameplateColorPicker.OnRightClick += (_, _) => { nameplateColorPicker.HasFocus = false; };
            nameplateColorPicker.OnFocusGain = () =>
            {
                JumpToModule("EngineModule");

                hslMenu.SetColorHSL(CustomizationDummy.Nameplate.TextColor.ToScaledHSL(luminanceSliderFactor));
                hslMenu.CaptureCurrentColor();
            };

            nameplatePicker = new ColorPicker(
               context: ColorPicker.ContextType.Nameplate,
               panel: nameplateColorPicker
            );

            nameplateConfigPanel.Append(nameplateColorPicker);

            alignLeft = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "AlignLeft"))
            {
                VAlign = 0.5f,
                HAlign = 0f,
                Left = new(0f, 0.56f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignLeft"),
                FocusContext = "HorizontalAlignment",
                CheckInteractible = AlignButtonInteractible,
                OnFocusGain = () =>
                {
                    JumpToModule("EngineModule");
                    CustomizationDummy.Nameplate.HAlign = TextHorizontalAlign.Left;
                }
            };
            alignLeft.OnLeftClick += (_, _) => alignLeft.HasFocus = true;

            nameplateConfigPanel.Append(alignLeft);

            alignCenterHorizontal = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "AlignCenterHorizontal"))
            {
                VAlign = 0.5f,
                Left = new(0f, 0.628f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignCenterHorizontal"),
                FocusContext = "HorizontalAlignment",
                CheckInteractible = AlignButtonInteractible,
                OnFocusGain = () =>
                {
                    JumpToModule("EngineModule");
                    CustomizationDummy.Nameplate.HAlign = TextHorizontalAlign.Center;
                }
            };
            alignCenterHorizontal.OnLeftClick += (_, _) => alignCenterHorizontal.HasFocus = true;
            nameplateConfigPanel.Append(alignCenterHorizontal);

            alignRight = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "AlignRight"))
            {
                VAlign = 0.5f,
                Left = new(0f, 0.696f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignRight"),
                FocusContext = "HorizontalAlignment",
                CheckInteractible = AlignButtonInteractible,
                OnFocusGain = () =>
                {
                    JumpToModule("EngineModule");
                    CustomizationDummy.Nameplate.HAlign = TextHorizontalAlign.Right;
                }
            };
            alignRight.OnLeftClick += (_, _) => alignRight.HasFocus = true;
            nameplateConfigPanel.Append(alignRight);

            alignTop = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "AlignTop"))
            {
                VAlign = 0.5f,
                Left = new(0f, 0.78f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignTop"),
                FocusContext = "VerticalAlignment",
                CheckInteractible = AlignButtonInteractible,
                OnFocusGain = () =>
                {
                    JumpToModule("EngineModule");
                    CustomizationDummy.Nameplate.VAlign = TextVerticalAlign.Top;
                }
            };
            alignTop.OnLeftClick += (_, _) => alignTop.HasFocus = true;
            nameplateConfigPanel.Append(alignTop);

            alignCenterVertical = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "AlignCenterVertical"))
            {
                VAlign = 0.5f,
                Left = new(0f, 0.848f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignCenterVertical"),
                FocusContext = "VerticalAlignment",
                CheckInteractible = AlignButtonInteractible,
                OnFocusGain = () =>
                {
                    JumpToModule("EngineModule");
                    CustomizationDummy.Nameplate.VAlign = TextVerticalAlign.Center;
                }
            };
            alignCenterVertical.OnLeftClick += (_, _) => alignCenterVertical.HasFocus = true;
            nameplateConfigPanel.Append(alignCenterVertical);

            alignBottom = new(ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "AlignBottom"))
            {
                VAlign = 0.5f,
                Left = new(0f, 0.917f),
                HoverText = Language.GetText("Mods.Macrocosm.UI.Common.AlignBottom"),
                FocusContext = "VerticalAlignment",
                CheckInteractible = AlignButtonInteractible,
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

        private bool AlignButtonInteractible() => !string.IsNullOrEmpty(CustomizationDummy.Nameplate.Text);

        private UIListScrollablePanel CreateDecalConfigPanel()
        {
            decalConfigPanel = new()
            {
                Width = new(0, 0.99f),
                Height = new(0, 0.245f),
                HAlign = 0.5f,
                Top = new(0f, 0.36f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                ListPadding = 0f,
                ListOuterPadding = 2f,
                ScrollbarHeight = new(0f, 0.9f),
                ScrollbarHAlign = 0.99f,
                ListWidthWithScrollbar = new(0, 1f),
                ListWidthWithoutScrollbar = new(0, 1f)
            };
            decalConfigPanel.SetPadding(0f);

            var decals = DecalManager.GetUnlockedDecals(currentModuleName);
            int count = decals.Count;

            int iconsPerRow = 9;
            int rowsWithoutScrollbar = 4;
            float iconSize = 51f;
            float iconOffsetTop = 8f;
            float iconOffsetLeft = 9f;

            if (count > iconsPerRow * rowsWithoutScrollbar)
            {
                iconSize -= 2f;
                iconOffsetLeft -= 1f;
            }

            UIElement decalIconContainer = new()
            {
                Width = new(0f, 1f),
                Height = new(iconSize * (count / iconsPerRow + ((count % iconsPerRow != 0) ? 1 : 0)), 0f),
            };

            decalConfigPanel.Add(decalIconContainer);
            decalIconContainer.SetPadding(0f);

            for (int i = 0; i < count; i++)
            {
                Decal decal = decals[i];
                UIDecalIcon icon = new(decal)
                {
                    Left = new((i % iconsPerRow) * iconSize + iconOffsetLeft, 0f),
                    Top = new((i / iconsPerRow) * iconSize + iconOffsetTop, 0f)
                };

                icon.Activate();
                decalIconContainer.Append(icon);
            }

            foreach (var icon in decalConfigPanel.OfType<UIDecalIcon>().ToList())
                icon.OnLeftClick += (_, icon) => SelectDecal(icon as UIDecalIcon);

            return decalConfigPanel;
        }

        private UIPanel CreatePatternConfigPanel()
        {
            patternConfigPanel = new()
            {
                Width = new(0, 0.99f),
                Height = new(0, 0.375f),
                HAlign = 0.5f,
                Top = new(0f, 0.62f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            patternConfigPanel.SetPadding(0f);

            patternSelector = CreatePatternSelector(currentModuleName);
            foreach (var icon in patternSelector.OfType<UIPatternIcon>().ToList())
                icon.OnLeftClick += (_, icon) => PatternIconOnClick(icon as UIPatternIcon);

            patternConfigPanel.Append(patternSelector);

            return patternConfigPanel;
        }

        private void PatternIconOnClick(UIPatternIcon icon)
        {
            if(icon.Pattern.Name != (CurrentModule?.Pattern.Name ?? ""))
            {
                SelectPattern(icon);
                RefreshPatternColorPickers();
            }
        }

        public static UIListScrollablePanel CreatePatternSelector(string moduleName)
        {
            UIListScrollablePanel listPanel = new()
            {
                Width = new(0, 1f),
                Height = new(0, 0.8f),
                HAlign = 0.5f,
                Top = new(0f, 0.2f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                ListPadding = 0f,
                ListOuterPadding = 2f,
                ScrollbarHeight = new(0f, 0.9f),
                ScrollbarHAlign = 0.99f,
                ListWidthWithScrollbar = new(0, 1f),
                ListWidthWithoutScrollbar = new(0, 1f)
            };
            listPanel.SetPadding(0f);

            var patterns = PatternManager.GetAll(context: moduleName, unlocked: true).ToList();
            int count = patterns.Count;

            int iconsPerRow = 9;
            int rowsWithoutScrollbar = 4;
            float iconSize = 52f;
            float iconOffsetTop = 8f;
            float iconOffsetLeft = 9f;

            if (count > iconsPerRow * rowsWithoutScrollbar)
            {
                iconSize -= 2f;
                iconOffsetLeft -= 1f;
            }

            UIElement patternIconContainer = new()
            {
                Width = new(0f, 1f),
                Height = new(iconSize * (count / iconsPerRow + ((count % iconsPerRow != 0) ? 1 : 0)), 0f),
            };

            listPanel.Add(patternIconContainer);
            patternIconContainer.SetPadding(0f);

            for (int i = 0; i < count; i++)
            {
                Pattern pattern = patterns[i].Clone();
                UIPatternIcon icon = new(pattern)
                {
                    Left = new((i % iconsPerRow) * iconSize + iconOffsetLeft, 0f),
                    Top = new((i / iconsPerRow) * iconSize + iconOffsetTop, 0f)
                };

                icon.Activate();
                patternIconContainer.Append(icon);
            }

            return listPanel;
        }
        #endregion
    }
}
