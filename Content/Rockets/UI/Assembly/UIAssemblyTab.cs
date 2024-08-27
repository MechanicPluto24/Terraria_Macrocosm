using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
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

namespace Macrocosm.Content.Rockets.UI.Assembly
{
    public class UIAssemblyTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();
        public LaunchPad LaunchPad { get; set; } = new();
        private Inventory Inventory => LaunchPad.Inventory;

        private Dictionary<string, UIModuleAssemblyElement> assemblyElements;
        private UIRocketBlueprint uIRocketBlueprint;
        private UIPanelIconButton assembleButton;
        private UIInfoElement compass;
        private UIInputTextBox nameTextBox;
        private UIPanelIconButton nameAcceptResetButton;

        public UIAssemblyTab()
        {
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

            uIRocketBlueprint = new();
            Append(uIRocketBlueprint);

            nameTextBox = CreateNameTextBox();
            compass = CreateCompassCoordinatesInfo();
            assembleButton = CreateAssembleButton();
            assemblyElements = CreateAssemblyElements();

            UpdateBlueprint();
        }

        public void OnTabOpen()
        {
        }

        public void OnTabClose()
        {
        }

        private void AssembleRocket()
        {
            Rocket.Create(LaunchPad.CenterWorld - new Vector2(Rocket.Width / 2f - 8, Rocket.Height - 18));
            Check(consume: true);
        }

        private bool CheckInteractible() => Check(consume: false) && !LaunchPad.Rocket.Active && RocketManager.ActiveRocketCount < RocketManager.MaxRockets;

        private bool Check(bool consume)
        {
            bool met = true;

            foreach (var kvp in Rocket.Modules)
            {
                RocketModule module = kvp.Value;

                if (module.Recipe.Linked)
                    met &= assemblyElements[module.Recipe.LinkedResult.Name].Check(consume);
                else
                    met &= assemblyElements[module.Name].Check(consume);
            }

            return met;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateTextbox();
            UpdateBlueprint();

            Inventory.ActiveInventory = LaunchPad.Inventory;
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

            foreach (var kvp in Rocket.Modules)
            {
                RocketModule module = kvp.Value;

                if (Rocket.Active)
                {
                    module.IsBlueprint = false;
                }
                else
                {
                    if (module.Recipe.Linked)
                        module.IsBlueprint = !assemblyElements[module.Recipe.LinkedResult.Name].Check(consume: false);
                    else
                        module.IsBlueprint = !assemblyElements[module.Name].Check(consume: false);
                }
            }
        }

        private UIInputTextBox CreateNameTextBox()
        {
            nameTextBox = new(Language.GetTextValue("Mods.Macrocosm.Common.LaunchPad"))
            {
                Width = new(0f, 0.3f),
                Height = new(0f, 0.05f),
                Top = new(0, 0.065f),
                Left = new(0, 0.125f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                HoverBorderColor = UITheme.Current.ButtonHighlightStyle.BorderColor,
                TextMaxLength = 18,
                TextScale = 0.8f,
                FocusContext = "TextBox",
                OnFocusGain = () =>
                {
                    nameAcceptResetButton.SetPanelTextures(
                        ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkWhite"),
                        ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkBorderHighlight"),
                        ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "CheckmarkBorderHighlight")
                    );
                },
                OnFocusLost = () =>
                {
                    nameAcceptResetButton.SetPanelTextures(
                        ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite"),
                        ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight"),
                        ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight")
                    );
                }
            };
            Append(nameTextBox);

            nameAcceptResetButton = new
            (
                Macrocosm.EmptyTex,
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetWhite"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight"),
                ModContent.Request<Texture2D>(Macrocosm.SymbolsPath + "ResetBorderHighlight")
            )
            {
                Width = new(20, 0),
                Height = new(20, 0),
                Top = new(0, 0.0745f),
                Left = new(0, 0.388f),
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
                Left = new(0, 0.18f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            Append(compass);
            compass.Activate();

            return compass;
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
                Top = new(0, 0.895f),
                Left = new(0, 0.2f),
                CheckInteractible = CheckInteractible,
                GrayscaleIconIfNotInteractible = true,
                GetIconPosition = (dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.2f, dimensions.Height * 0.5f)
            };
            assembleButton.SetText(new(Language.GetText("Mods.Macrocosm.UI.LaunchPad.Assemble")), 0.8f, darkenTextIfNotInteractible: true);
            assembleButton.OnLeftClick += (_, _) => AssembleRocket();
            Append(assembleButton);

            return assembleButton;
        }

        private Dictionary<string, UIModuleAssemblyElement> CreateAssemblyElements()
        {
            assemblyElements = new();

            int slotCount = 0;
            int assemblyElementCount = 0;
            foreach (var kvp in Rocket.Modules)
            {
                RocketModule module = kvp.Value;

                if (!module.Recipe.Linked)
                {
                    List<UIInventorySlot> slots = new();
                    for (int i = 0; i < module.Recipe.Count(); i++)
                    {
                        AssemblyRecipeEntry recipeEntry = module.Recipe[i];
                        UIInventorySlot slot = Inventory.ProvideItemSlot(slotCount++);

                        if (recipeEntry.ItemType.HasValue)
                            slot.AddReserved(recipeEntry.ItemType.Value, recipeEntry.Description, GetBlueprintTexture(recipeEntry.ItemType.Value));
                        else
                            slot.AddReserved(recipeEntry.ItemCheck, recipeEntry.Description, GetBlueprintTexture(recipeEntry.Description.Key));

                        if (recipeEntry.RequiredAmount > 1)
                        {
                            UIText amountRequiredText = new("x" + recipeEntry.RequiredAmount.ToString(), textScale: 0.8f)
                            {
                                Top = new(0, 0.98f),
                                HAlign = 0.5f
                            };
                            slot.Append(amountRequiredText);
                        }

                        slot.SizeLimit += 8;
                        slots.Add(slot);
                    }

                    UIModuleAssemblyElement assemblyElement = new(module, slots);
                    assemblyElements[module.Name] = assemblyElement;

                    assemblyElement.Top = new(0, 0.14f + 0.185f * assemblyElementCount++);
                    assemblyElement.Left = new(0, 0.15f);

                    Append(assemblyElement);
                }
            }

            foreach (var kvp in Rocket.Modules)
            {
                RocketModule module = kvp.Value;

                if (module.Recipe.Linked)
                {
                    assemblyElements[module.Recipe.LinkedResult.Name].LinkedModules.Add(module);
                }
            }

            return assemblyElements;
        }
        private Asset<Texture2D> GetBlueprintTexture(int itemType)
        {
            Item item = ContentSamples.ItemsByType[itemType];

            if (item.ModItem is ModItem modItem)
                return ModContent.RequestIfExists(modItem.Texture + "_Blueprint", out Asset<Texture2D> blueprint) ? blueprint : null;

            return null;
        }

        private Asset<Texture2D> GetBlueprintTexture(string key)
        {
            string keySuffix = key[(key.LastIndexOf('.') + 1)..];

            if (ModContent.RequestIfExists(Macrocosm.TexturesPath + "UI/Blueprints/" + keySuffix, out Asset<Texture2D> blueprint))
                return blueprint;

            return null;
        }
    }
}
