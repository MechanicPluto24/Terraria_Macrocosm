using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Items.Materials.Tech;
using Macrocosm.Content.Rockets.LaunchPads;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Macrocosm.Content.Items.Blocks;
using System;
using System.Linq;
using Macrocosm.Content.Rockets.Modules;
using System.Collections.Generic;
using rail;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.UI.Assembly
{
    public class UIAssemblyTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();
        public LaunchPad LaunchPad { get; set; } = new();
        private Inventory Inventory => LaunchPad.Inventory;

        private Dictionary<string, UIModuleAssemblyElement> assemblyElements = new();
        private UIRocketBlueprint uIRocketBlueprint;
        private UIPanelIconButton assembleButton;

        public UIAssemblyTab()
        {
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            SetPadding(3f);

            BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
            BorderColor = UITheme.Current.TabStyle.BorderColor;

            assembleButton = new(
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Symbols/Wrench"),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/WidePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, 0.86f),
                Left = new(0, 0.2f),
                CheckInteractible = CheckInteractible,
                GrayscaleIconIfNotInteractible = true,
                GetIconPosition = (dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.2f, dimensions.Height * 0.5f)
            };
            assembleButton.SetText(new(Language.GetText("Mods.Macrocosm.UI.Rocket.Assembly.Assemble")), 0.8f);
            assembleButton.OnLeftClick += (_, _) => AssembleRocket();
            Append(assembleButton);

            uIRocketBlueprint = new();
            Append(uIRocketBlueprint);

            AddAssemblyElements();
        }

        public void OnTabOpen()
        {
        }

        public void OnTabClose()
        {
        }

        private void AssembleRocket()
        {
            Rocket.Create(LaunchPad.Position - new Vector2(Rocket.Width / 2f - 8, Rocket.Height - 18));
            Check(consume: true);
        }

        private bool CheckInteractible()
        {
            return Check(consume: false) && !LaunchPad.Rocket.Active && RocketManager.ActiveRocketCount < RocketManager.MaxRockets;
        }

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

        private void AddAssemblyElements()
        {
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

                    assemblyElement.Top = new(0, 0.1f + 0.185f * assemblyElementCount++);
                    assemblyElement.Left = new(0, 0.15f);

                    Append(assemblyElement);
                }
            }
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
