using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI.Rockets.Assembly
{
    public class UIAssemblyTab : UIElement, ITabUIElement
    {
        public LaunchPad LaunchPad { get; init; } = new();
        public Rocket Rocket => LaunchPad.Rocket;
        private Inventory Inventory => LaunchPad.Inventory;

        private Dictionary<string, UIModuleAssemblyElement> assemblyElements;
        private UIRocketBlueprint uIRocketBlueprint;

        private UIPanel uiAssemblyPanel;

        private UIPanelIconButton assembleButton;
        private UIPanelIconButton dissasembleButton;

        private UIInfoElement compass;
        private UIInputTextBox nameTextBox;
        private UIPanelIconButton nameAcceptResetButton;

        private UIInfoElement configurationSelector;
        private UIHoverImageButton configurationLeftArrow;
        private UIHoverImageButton configurationRightArrow;

        public UIAssemblyTab()
        {
        }

        public UIAssemblyTab(LaunchPad launchPad)
        {
            LaunchPad = launchPad;
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;
            SetPadding(0f);

            uIRocketBlueprint = new()
            {
                Rocket = LaunchPad.Rocket,

                Width = new(0, 0.5f),
                Height = new(0, 1f),
                Left = new(0, 0.5f),
                HAlign = 0f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            Append(uIRocketBlueprint);

            uiAssemblyPanel = new()
            {
                Width = new(0, 0.5f),
                Height = new(0, 1f),
                Left = new(0, 0f),
                HAlign = 0f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            Append(uiAssemblyPanel);

            nameTextBox = CreateNameTextBox();
            uiAssemblyPanel.Append(nameTextBox);

            nameAcceptResetButton = CreateNameAcceptResetButton();
            uiAssemblyPanel.Append(nameAcceptResetButton);

            compass = CreateCompassCoordinatesInfo();
            uiAssemblyPanel.Append(compass);
            compass.Activate();

            configurationSelector = CreateConfigurationSelector();
            uiAssemblyPanel.Append(configurationSelector);

            configurationLeftArrow = CreateLeftArrow();
            uiAssemblyPanel.Append(configurationLeftArrow);
            configurationRightArrow = CreateRightArrow();
            uiAssemblyPanel.Append(configurationRightArrow);

            assembleButton = CreateAssembleButton();
            uiAssemblyPanel.Append(assembleButton);
            dissasembleButton = CreateDissasembleButton();

            assemblyElements = CreateAssemblyElements();

            UpdateBlueprint();
        }

        public void OnTabOpen()
        {
            Main.stackSplit = 600;
        }

        public void OnTabClose()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateTextbox();
            UpdateBlueprint();
            UpdateAssembleButton();
            configurationSelector.SetText(Language.GetText("Mods.Macrocosm.UI.LaunchPad.Configurations." + LaunchPad.CurrentConfiguration.ToString()));

            Inventory.ActiveInventory = LaunchPad.Inventory;
        }

        private void UpdateAssembleButton()
        {
            if (LaunchPad.HasActiveRocket)
            {
                if (uiAssemblyPanel.HasChild(assembleButton))
                    uiAssemblyPanel.ReplaceChildWith(assembleButton, dissasembleButton = CreateDissasembleButton());
            }
            else
            {
                if (uiAssemblyPanel.HasChild(dissasembleButton))
                    uiAssemblyPanel.ReplaceChildWith(dissasembleButton, assembleButton = CreateAssembleButton());
            }
        }

        private void UpdateTextbox()
        {
            if (!nameTextBox.HasFocus)
                nameTextBox.Text = LaunchPad.DisplayName;

            if (nameTextBox.Text == Language.GetTextValue("Mods.Macrocosm.Common.LaunchPad") && !nameTextBox.HasFocus)
                nameTextBox.TextColor = Color.Gray;
            else
                nameTextBox.TextColor = Color.White;
        }

        private void UpdateBlueprint()
        {
            uIRocketBlueprint.Rocket = Rocket;
        }

        private UIInputTextBox CreateNameTextBox()
        {
            nameTextBox = new(Language.GetTextValue("Mods.Macrocosm.Common.LaunchPad"))
            {
                Width = new(0f, 0.8f),
                Height = new(0f, 0.05f),
                Top = new(0, 0.0115f),
                HAlign = 0.5f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                HoverBorderColor = UITheme.Current.PanelButtonStyle.BorderColorHighlight,
                TextMaxLength = 18,
                TextScale = 0.9f,
                FocusContext = "TextBox",
                OnFocusGain = () =>
                {
                    nameAcceptResetButton.SetPanelTextures(
                        ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "CheckmarkWhite", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "CheckmarkBorderHighlight", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "CheckmarkBorderHighlight", AssetRequestMode.ImmediateLoad)
                    );
                },
                OnFocusLost = () =>
                {
                    nameAcceptResetButton.SetPanelTextures(
                        ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "ResetWhite", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad)
                    );
                }
            };

            return nameTextBox;
        }

        private UIPanelIconButton CreateNameAcceptResetButton()
        {
            nameAcceptResetButton = new
            (
                Macrocosm.EmptyTex,
                ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "ResetWhite", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad)
            )
            {
                Width = new(20, 0),
                Height = new(20, 0),
                Top = new(0, 0.02f),
                Left = new(0, 0.8f),
                BackPanelColor = Color.White,
                FocusedBackPanelColor = Color.White,
                BackPanelBorderColor = Color.Transparent,
                BackPanelHoverBorderColor = Color.White
            };
            nameAcceptResetButton.OnLeftClick += (_, _) =>
            {
                if (nameTextBox.HasFocus)
                {
                    LaunchPad.CustomName = nameTextBox.Text;
                    nameTextBox.HasFocus = false;
                }
                else
                {
                    LaunchPad.CustomName = "";
                    nameTextBox.Text = Language.GetTextValue("Mods.Macrocosm.Common.LaunchPad");
                }

            };

            return nameAcceptResetButton;
        }

        private UIInfoElement CreateCompassCoordinatesInfo()
        {
            compass = new(new LocalizedColorScaleText(Language.GetText(LaunchPad.CompassCoordinates), scale: 0.9f), TextureAssets.Item[ItemID.Compass])
            {
                Width = new(180, 0),
                Height = new(34, 0),
                Top = new(0, 0.065f),
                HAlign = 0.5f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };

            return compass;
        }

        private UIInfoElement CreateConfigurationSelector()
        {
            configurationSelector = new UIInfoElement(LocalizedText.Empty, ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "Wrench", AssetRequestMode.ImmediateLoad))
            {
                Width = new(180, 0),
                Height = new(30, 0),
                Top = new(0, 0.125f),
                HAlign = 0.5f,
                IconHAlign = 0.15f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };

            return configurationSelector;
        }

        private UIHoverImageButton CreateLeftArrow()
        {
            configurationLeftArrow = new(
                ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "ShortArrowPlain", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
                LocalizedText.Empty,
                useThemeColors: true
            )
            {
                Top = new(0, 0.1175f),
                HAlign = 0.1675f,
                SpriteEffects = SpriteEffects.FlipHorizontally,
                Color = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                ColorHighlight = UITheme.Current.PanelStyle.BackgroundColor
            };

            configurationLeftArrow.OnLeftClick += (_, _) =>
            {
                LaunchPad.SwitchAssemblyModulesConfiguration(-1);
                RefreshAssemblyElements();
            };
            configurationLeftArrow.CheckInteractible = () => LaunchPad.SwitchAssemblyModulesConfiguration(-1, justCheck: true);
            configurationLeftArrow.SetVisibility(1f, 0f, 1f);
            return configurationLeftArrow;
        }

        private UIHoverImageButton CreateRightArrow()
        {
            configurationRightArrow = new(
                ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "ShortArrowPlain", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
                LocalizedText.Empty,
                useThemeColors: true
            )
            {
                Top = new(0, 0.1175f),
                HAlign = 0.8325f,
                Color = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                ColorHighlight = UITheme.Current.PanelStyle.BackgroundColor
            };

            configurationRightArrow.OnLeftClick += (_, _) =>
            {
                LaunchPad.SwitchAssemblyModulesConfiguration(1);
                RefreshAssemblyElements();
            };
            configurationRightArrow.CheckInteractible = () => LaunchPad.SwitchAssemblyModulesConfiguration(1, justCheck: true);
            configurationRightArrow.SetVisibility(1f, 0f, 1f);
            return configurationRightArrow;
        }

        private UIPanelIconButton CreateAssembleButton()
        {
            assembleButton = new
            (
                ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "Wrench"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, 0.91f),
                HAlign = 0.5f,
                CheckInteractible = LaunchPad.CanAssemble,
                GrayscaleIconIfNotInteractible = true,
                GetIconPosition = (dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.2f, dimensions.Height * 0.5f)
            };
            assembleButton.SetText(new(Language.GetText("Mods.Macrocosm.UI.LaunchPad.Assemble")), 0.8f, darkenTextIfNotInteractible: true);
            assembleButton.OnLeftClick += (_, _) => LaunchPad.Assemble();

            return assembleButton;
        }

        private UIPanelIconButton CreateDissasembleButton()
        {
            dissasembleButton = new
            (
                ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "Wrench"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, 0.91f),
                HAlign = 0.5f,
                CheckInteractible = LaunchPad.CanDisassemble,
                GrayscaleIconIfNotInteractible = true,
                GetIconPosition = (dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.2f, dimensions.Height * 0.5f)
            };
            dissasembleButton.SetText(new(Language.GetText("Mods.Macrocosm.UI.LaunchPad.Disassemble"), scale: 0.8f), 0.8f, darkenTextIfNotInteractible: true);
            dissasembleButton.OnLeftClick += (_, _) => LaunchPad.Disassemble();

            return dissasembleButton;
        }

        private Dictionary<string, UIModuleAssemblyElement> CreateAssemblyElements()
        {
            assemblyElements = new();

            foreach (RocketModule m in RocketModule.Templates.OrderBy(m => m.Slot))
            {
                RocketModule module = m.Clone();
                if (!module.Recipe.Linked)
                {
                    UIModuleAssemblyElement assemblyElement = new(module, LaunchPad)
                    {
                        Top = new(0, 0.19f + 0.175f * (int)module.Slot),
                        HAlign = 0.5f,
                    };

                    assemblyElements[module.Name] = assemblyElement;
                    if (Rocket.Modules.Select(m => m.Name).Contains(module.Name))
                    {
                        uiAssemblyPanel.Append(assemblyElement);
                        assemblyElement.Activate();
                    }
                }
            }

            return assemblyElements;
        }

        public void RefreshAssemblyElements()
        {
            this.RemoveAllChildrenWhere(element => element is UIModuleAssemblyElement);
            assemblyElements = CreateAssemblyElements();
        }
    }
}
