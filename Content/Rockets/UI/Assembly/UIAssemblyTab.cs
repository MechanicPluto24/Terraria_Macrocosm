using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
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
        private UIPanelIconButton dissasembleButton;

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

        private bool CheckAssembleRecipes(bool consume)
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


        private void AssembleRocket()
        {
            CheckAssembleRecipes(consume: true);
            Rocket = Rocket.Create(LaunchPad.CenterWorld - new Vector2(Rocket.Width / 2f - 8, Rocket.Height - 18));

            foreach (var kvp in Rocket.Modules)
            {
                RocketModule module = kvp.Value;

                if (module.Recipe.Linked)
                    continue;

                for (int i = 0; i < module.Recipe.Count(); i++)
                {
                    AssemblyRecipeEntry recipeEntry = module.Recipe[i];
                    Item item = null;
                    if (recipeEntry.ItemType.HasValue)
                    {
                        item = new(recipeEntry.ItemType.Value, recipeEntry.RequiredAmount);
                    }
                    else
                    {
                        int defaultType = ContentSamples.ItemsByType.Values.FirstOrDefault((item) => recipeEntry.ItemCheck(item)).type;
                        item = new(defaultType, recipeEntry.RequiredAmount);
                    }

                    for (int particle = 0; particle < item.stack; particle++)
                    {
                        if (Main.rand.NextBool(6))
                            Particle.Create<ItemTransferParticle>((p) =>
                            {
                                p.StartPosition = LaunchPad.CenterWorld;
                                p.EndPosition = module.Center + Main.rand.NextVector2Circular(64, 64);
                                p.ItemType = item.type;
                                p.TimeToLive = Main.rand.Next(40, 60);
                            });
                    }

                }
            }
        }

        private void DisassembleRocket()
        {
            int slot = 0;
            foreach (var kvp in LaunchPad.Rocket.Modules)
            {
                RocketModule module = kvp.Value;

                if (module.Recipe.Linked)
                    continue;

                for (int i = 0; i < module.Recipe.Count(); i++)
                {
                    AssemblyRecipeEntry recipeEntry = module.Recipe[i];
                    Item item = null;
                    if (recipeEntry.ItemType.HasValue)
                    {
                        item = new(recipeEntry.ItemType.Value, recipeEntry.RequiredAmount);
                    }
                    else
                    {
                        int defaultType = ContentSamples.ItemsByType.Values.FirstOrDefault((item) => recipeEntry.ItemCheck(item)).type;
                        item = new(defaultType, recipeEntry.RequiredAmount);
                    }

                    bool spawnParticle = true;
                    Item particleItem = item.Clone();
                    if (!LaunchPad.Inventory.TryPlacingItemInSlot(item, slot, sound: true))
                    {
                        Main.LocalPlayer.QuickSpawnItem(item.GetSource_DropAsItem("Launchpad"), item.type, item.stack);
                        spawnParticle = false;
                    }

                    if (spawnParticle)
                    {
                        for (int particle = 0; particle < particleItem.stack; particle++)
                        {
                            if (Main.rand.NextBool(6))
                                Particle.Create<ItemTransferParticle>((p) =>
                                {
                                    p.StartPosition = module.Center + Main.rand.NextVector2Circular(64, 64);
                                    p.EndPosition = LaunchPad.CenterWorld;
                                    p.ItemType = particleItem.type;
                                    p.TimeToLive = Main.rand.Next(50, 70);
                                });
                        }
                    }

                    slot++;
                }
            }

            LaunchPad.Rocket.Despawn();
            Rocket = new();
        }

        private bool CheckAssembleInteractible() => CheckAssembleRecipes(consume: false) && !LaunchPad.Rocket.Active && RocketManager.ActiveRocketCount < RocketManager.MaxRockets;
        private bool CheckDissasembleInteractible() => LaunchPad.HasRocket;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateTextbox();
            UpdateBlueprint();
            UpdateAssembleButton();

            Inventory.ActiveInventory = LaunchPad.Inventory;
        }

        private void UpdateAssembleButton()
        {
            if (LaunchPad.HasRocket)
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
                Left = new(0, 0.14f),
                CheckInteractible = CheckAssembleInteractible,
                GrayscaleIconIfNotInteractible = true,
                GetIconPosition = (dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.2f, dimensions.Height * 0.5f)
            };
            assembleButton.SetText(new(Language.GetText("Mods.Macrocosm.UI.LaunchPad.Assemble")), 0.8f, darkenTextIfNotInteractible: true);
            assembleButton.OnLeftClick += (_, _) => AssembleRocket();

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
                Top = new(0, 0.895f),
                Left = new(0, 0.14f),
                CheckInteractible = CheckDissasembleInteractible,
                GrayscaleIconIfNotInteractible = true,
                GetIconPosition = (dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.2f, dimensions.Height * 0.5f)
            };
            dissasembleButton.SetText(new(Language.GetText("Mods.Macrocosm.UI.LaunchPad.Disassemble"), scale: 0.8f), 0.8f, darkenTextIfNotInteractible: true);
            dissasembleButton.OnLeftClick += (_, _) => DisassembleRocket();

            return dissasembleButton;
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

                        if (recipeEntry.ItemType.HasValue)
                            Inventory.SetReserved(slotCount, recipeEntry.ItemType.Value, recipeEntry.Description, GetBlueprintTexture(recipeEntry.ItemType.Value));
                        else
                            Inventory.SetReserved(slotCount, recipeEntry.ItemCheck, recipeEntry.Description, GetBlueprintTexture(recipeEntry.Description.Key));

                        UIInventorySlot slot = Inventory.ProvideItemSlot(slotCount++);
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
                    assemblyElement.Left = new(0, 0.08f);

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
