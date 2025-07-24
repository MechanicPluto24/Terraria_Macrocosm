using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
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

namespace Macrocosm.Content.Rockets.UI.Assembly;

public class UIAssemblyTab : UIPanel, ITabUIElement
{
    public LaunchPad LaunchPad { get; init; } = new();
    public Rocket Rocket => LaunchPad.Rocket;
    private Inventory Inventory => LaunchPad.Inventory;

    private Dictionary<string, UIModuleAssemblyElement> assemblyElements;
    private UIRocketBlueprint uIRocketBlueprint;

    private UIPanelIconButton assembleButton;
    private UIPanelIconButton dissasembleButton;

    private UIInfoElement compass;
    private UIInputTextBox nameTextBox;
    private UIPanelIconButton nameAcceptResetButton;

    private UIPanel configurationSelector;
    private UIText configurationText;
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

        SetPadding(4f);

        BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
        BorderColor = UITheme.Current.TabStyle.BorderColor;

        uIRocketBlueprint = new()
        {
            Rocket = LaunchPad.Rocket
        };
        Append(uIRocketBlueprint);

        nameTextBox = CreateNameTextBox();
        compass = CreateCompassCoordinatesInfo();

        configurationSelector = CreateConfigurationSelector();

        assembleButton = CreateAssembleButton();
        Append(assembleButton);
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
        configurationText.SetText(LaunchPad.CurrentConfiguration.ToString());

        Inventory.ActiveInventory = LaunchPad.Inventory;
    }

    private void UpdateAssembleButton()
    {
        if (LaunchPad.HasActiveRocket)
        {
            if (HasChild(assembleButton))
                this.ReplaceChildWith(assembleButton, dissasembleButton = CreateDissasembleButton());
        }
        else
        {
            if (HasChild(dissasembleButton))
                this.ReplaceChildWith(dissasembleButton, assembleButton = CreateAssembleButton());
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
            Width = new(0f, 0.3f),
            Height = new(0f, 0.05f),
            Top = new(0, 0.065f),
            Left = new(0, 0.1f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            HoverBorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor,
            TextMaxLength = 18,
            TextScale = 0.8f,
            FocusContext = "TextBox",
            OnFocusGain = () =>
            {
                nameAcceptResetButton.SetPanelTextures(
                    ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkWhite", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkBorderHighlight", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkBorderHighlight", AssetRequestMode.ImmediateLoad)
                );
            },
            OnFocusLost = () =>
            {
                nameAcceptResetButton.SetPanelTextures(
                    ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad)
                );
            }
        };
        Append(nameTextBox);

        nameAcceptResetButton = new
        (
            Macrocosm.EmptyTex,
            ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight", AssetRequestMode.ImmediateLoad)
        )
        {
            Width = new(20, 0),
            Height = new(20, 0),
            Top = new(0, 0.075f),
            Left = new(0, 0.36f),
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
        Append(nameAcceptResetButton);

        return nameTextBox;
    }

    private UIInfoElement CreateCompassCoordinatesInfo()
    {
        compass = new(Language.GetTextValue(LaunchPad.CompassCoordinates), TextureAssets.Item[ItemID.Compass])
        {
            Width = new(160, 0),
            Height = new(30, 0),
            Top = new(0, 0.016f),
            Left = new(0, 0.13f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };
        Append(compass);
        compass.Activate();

        return compass;
    }

    private UIPanel CreateConfigurationSelector()
    {
        configurationSelector = new()
        {
            Width = new(160, 0),
            Height = new(40, 0),
            Top = new(0, 0.121f),
            Left = new(0, 0.13f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };

        configurationText = new UIText("AAAA", 0.8f)
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            TextColor = Color.White
        };
        configurationSelector.Append(configurationText);

        configurationLeftArrow = new(
            ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
            LocalizedText.Empty
        )
        {
            Left = new(-8, 0f),
            VAlign = 0.5f,
            SpriteEffects = SpriteEffects.FlipHorizontally
        };

        configurationLeftArrow.OnLeftClick += (_, _) =>
        {
            LaunchPad.SwitchAssemblyModulesConfiguration(-1);
            RefreshAssemblyElements();
        };
        configurationLeftArrow.CheckInteractible = () => LaunchPad.SwitchAssemblyModulesConfiguration(-1, justCheck: true);
        configurationLeftArrow.SetVisibility(1f, 0f, 1f);
        configurationSelector.Append(configurationLeftArrow);

        configurationRightArrow = new(
            ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", AssetRequestMode.ImmediateLoad),
            LocalizedText.Empty
        )
        {
            Left = new(0, 0.8f),
            VAlign = 0.5f
        };

        configurationRightArrow.OnLeftClick += (_, _) =>
        {
            LaunchPad.SwitchAssemblyModulesConfiguration(1);
            RefreshAssemblyElements();
        };
        configurationRightArrow.CheckInteractible = () => LaunchPad.SwitchAssemblyModulesConfiguration(1, justCheck: true);
        configurationRightArrow.SetVisibility(1f, 0f, 1f);
        configurationSelector.Append(configurationRightArrow);

        Append(configurationSelector);
        return configurationSelector;
    }

    private UIPanelIconButton CreateAssembleButton()
    {
        assembleButton = new
        (
            ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "Wrench"),
            ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanel", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelBorder", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelHoverBorder", AssetRequestMode.ImmediateLoad)
        )
        {
            Top = new(0, 0.91f),
            Left = new(0, 0.14f),
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
            ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "Wrench"),
            ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanel", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelBorder", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelHoverBorder", AssetRequestMode.ImmediateLoad)
        )
        {
            Top = new(0, 0.91f),
            Left = new(0, 0.14f),
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
                    Left = new(0, 0.0485f)
                };

                assemblyElements[module.Name] = assemblyElement;
                if (Rocket.Modules.Select(m => m.Name).Contains(module.Name))
                {
                    Append(assemblyElement);
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
