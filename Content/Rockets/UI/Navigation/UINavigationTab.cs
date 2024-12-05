using Macrocosm.Common.Players;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.UI.Cargo;
using Macrocosm.Content.Rockets.UI.Customization;
using Macrocosm.Content.Rockets.UI.Navigation.Checklist;
using Macrocosm.Content.Rockets.UI.Navigation.Info;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI.Navigation
{
    public class UINavigationTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        public UIRocketPreviewSmall CustomizationPreview { get; set; }
        public UICargoFuelPreview CargoPreview { get; set; }

        private UILaunchButton launchButton;
        private UINavigationPanel navigationPanel;

        private UIFlightChecklist flightChecklist;
        private UICommanderPanel commanderPanel;
        private UIListScrollablePanel worldInfoPanel;
        private UIListScrollablePanel launchLocationsList;

        private UINavigationTarget lastTarget;
        private UINavigationTarget target;

        private LaunchPad targetLaunchPad;
        private bool selectedSpawnLocation;

        private UILaunchDestinationInfoElement spawnInfoElement;

        public UINavigationTab()
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

            navigationPanel = new();
            Append(navigationPanel);
            navigationPanel.Activate();

            flightChecklist = CreateFlightChecklist();
            Append(flightChecklist);
            flightChecklist.Activate();

            commanderPanel = CreateCommanderPanel();
            Append(commanderPanel);
            commanderPanel.Activate();

            worldInfoPanel = CreateWorldInfoPanel(MacrocosmSubworld.CurrentID);
            Append(worldInfoPanel);
            worldInfoPanel.Activate();

            launchLocationsList = CreateLaunchLocationPanel(MacrocosmSubworld.CurrentID);
            Append(launchLocationsList);
            launchLocationsList.Activate();

            launchButton = new()
            {
                ZoomIn = navigationPanel.ZoomIn,
                Launch = () => Rocket.Launch(target.TargetID, targetLaunchPad)
            };
            Append(launchButton);
            launchButton.Activate();

            CustomizationPreview = new();
            Append(CustomizationPreview);
            CustomizationPreview.Activate();

            CargoPreview = new();
            Append(CargoPreview);
            CargoPreview.Activate();
        }

        public override void OnDeactivate()
        {
        }

        public void OnTabOpen()
        {
            CustomizationPreview.OnTabOpen();
            LaunchPadManager.OnLaunchpadCreation += LaunchPadManager_OnLaunchpadCreation;
        }

        public void OnTabClose()
        {
            LaunchPadManager.OnLaunchpadCreation -= LaunchPadManager_OnLaunchpadCreation;
        }

        private void LaunchPadManager_OnLaunchpadCreation(object sender, System.EventArgs e)
        {
            if (target is not null)
                CreateLaunchLocationPanel(target.TargetID);
        }

        public override void Update(GameTime gameTime)
        {
            lastTarget = target;
            target = navigationPanel.CurrentMap.GetSelectedTarget();
            Main.LocalPlayer.GetModPlayer<RocketPlayer>().TargetWorld = target is null ? "" : target.TargetID;

            base.Update(gameTime);

            UpdateWorldInfoPanel();
            UpdateLaunchLocationsList();
            UpdateChecklist();
            UpdateMapTarget();
            UpdateLaunchButton();
        }

        private void UpdateWorldInfoPanel()
        {
            if (target is not null)
            {
                if (target != lastTarget)
                {
                    launchLocationsList.ClearList();
                    CreateWorldInfoPanel(target.TargetID);
                }
            }
            else if (launchLocationsList.Any())
            {
                launchLocationsList.ClearList();
            }
        }

        private void UpdateLaunchLocationsList()
        {
            if (target is not null && target != lastTarget)
                CreateLaunchLocationPanel(target.TargetID);

            targetLaunchPad = null;
            selectedSpawnLocation = false;

            foreach (var lpInfo in launchLocationsList.OfType<UILaunchDestinationInfoElement>())
            {
                if (lpInfo.HasFocus)
                {
                    if (lpInfo.LaunchPad is not null)
                        targetLaunchPad = lpInfo.LaunchPad;
                    else
                        selectedSpawnLocation = true;

                    break;
                }
            }
        }

        private void UpdateChecklist()
        {
            flightChecklist.MapTarget = target;
            flightChecklist.TargetLaunchpad = targetLaunchPad;
            flightChecklist.SelectedSpawnLocation = selectedSpawnLocation;
        }

        private void UpdateMapTarget()
        {
            bool checklistResult = flightChecklist.Check();

            if (target is not null)
            {
                target.IsReachable = checklistResult;
                target.LaunchLocationSelected = targetLaunchPad is not null || selectedSpawnLocation;
            }
        }

        private void UpdateLaunchButton()
        {
            if (!flightChecklist.SelectedLaunchCondition.IsMet)
                launchButton.ButtonState = UILaunchButton.StateType.NoTarget;
            else if (navigationPanel.CurrentMap.HasNext)
                launchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
            else if (!flightChecklist.DifferentTargetLaunchCondition.IsMet)
                launchButton.ButtonState = UILaunchButton.StateType.DifferentTarget;
            else if (!flightChecklist.LaunchpadVacantCondition.IsMet)
                launchButton.ButtonState = UILaunchButton.StateType.Occupied;
            else if (!flightChecklist.AllMet)
                launchButton.ButtonState = UILaunchButton.StateType.CantReach;
            else if (Main.LocalPlayer.GetModPlayer<RocketPlayer>().IsCommander)
                launchButton.ButtonState = UILaunchButton.StateType.Launch;
            else
                launchButton.ButtonState = UILaunchButton.StateType.LaunchInactive;
        }

        private UIFlightChecklist CreateFlightChecklist()
        {
            flightChecklist = new UIFlightChecklist
            {
                Top = new(0, 0.365f),
                Width = new(0f, 0.31f),
                Left = new(0, 0.68f),
                Height = new(0, 0.45f),
                TitleHAlign = 0.65f
            };
            flightChecklist.SetPadding(2f);

            return flightChecklist;
        }

        private UICommanderPanel CreateCommanderPanel()
        {
            commanderPanel = new UICommanderPanel
            {
                Top = new(0f, 0.835f),
                Width = new(0f, 0.31f),
                Left = new(0, 0.68f),
                Height = new(0f, 0.15f),
            };
            commanderPanel.SetPadding(2f);

            return commanderPanel;
        }

        private UIListScrollablePanel CreateWorldInfoPanel(string subworldId)
        {
            subworldId = MacrocosmSubworld.SanitizeID(subworldId, out string modName);

            if (worldInfoPanel is null)
            {
                worldInfoPanel = new(new LocalizedColorScaleText(Language.GetText($"Mods.{modName}.Subworlds.{subworldId}.DisplayName"), scale: 1.2f))
                {
                    Width = new(0, 0.31f),
                    Height = new(0, 0.62f),
                    Left = new(0, 0.01f),
                    Top = new(0, 0.365f),
                    BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                    BorderColor = UITheme.Current.PanelStyle.BorderColor
                };
                worldInfoPanel.SetPadding(0f);
            }
            else
            {
                worldInfoPanel.Deactivate();
                worldInfoPanel.ClearList();
                worldInfoPanel.SetTitle(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.Subworlds." + subworldId + ".DisplayName"), scale: 1.2f));
            }

            LocalizedText flavorText = WorldInfo.GetFlavorText(subworldId);
            if (flavorText != LocalizedText.Empty && flavorText.Value != "default")
            {
                worldInfoPanel.Add(new UIDynamicTextPanel(new LocalizedColorScaleText(flavorText, Color.White, scale: 0.85f)));
                worldInfoPanel.AddHorizontalSeparator();
            }

            List<InfoElement> elements = WorldInfo.GetInfoElements(subworldId);

            if (elements is not null)
            {
                bool foundHazards = false;
                foreach (InfoElement element in elements)
                {
                    if (!foundHazards && element is HazardInfoElement)
                    {
                        worldInfoPanel.AddHorizontalSeparator();
                        foundHazards = true;
                    }

                    worldInfoPanel.Add(element.ProvideUI());
                }
            }

            worldInfoPanel.Activate();
            return worldInfoPanel;
        }

        private UIListScrollablePanel CreateLaunchLocationPanel(string subworldId)
        {
            if (launchLocationsList is null)
            {
                launchLocationsList = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.LaunchLocations"), scale: 1.2f))
                {
                    Top = new(0, 0.365f),
                    Width = new(0f, 0.34f),
                    HAlign = 0.5f,
                    Height = new(0, 0.505f),
                    BorderColor = UITheme.Current.PanelStyle.BorderColor,
                    BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
                };
                launchLocationsList.SetPadding(0f);
                launchLocationsList.TitleHAlign = 0.6f;
            }

            List<UILaunchDestinationInfoElement> vacant = new();
            List<UILaunchDestinationInfoElement> occupied = new();
            UILaunchDestinationInfoElement current = null;
            // Add the launchpads
            foreach (var launchPad in LaunchPadManager.GetLaunchPads(subworldId))
            {
                bool isCurrent = Rocket is not null && launchPad.RocketID == Rocket.WhoAmI;
                bool isOccupied = Rocket is not null && launchPad.HasRocket;

                UILaunchDestinationInfoElement infoElement = new(launchPad)
                {
                    FocusContext = "LaunchLocations",
                };
                infoElement.OnLeftClick += InfoElement_OnLeftClick;
                infoElement.OnRightClick += InfoElement_OnRightClick;

                if (!isCurrent)
                {
                    if (isOccupied)
                    {
                        infoElement.IsReachable = false;
                        occupied.Add(infoElement);
                    }
                    else
                    {
                        infoElement.IsReachable = true;
                        vacant.Add(infoElement);
                    }
                }
                else
                {
                    current = infoElement;
                    current.IsReachable = false;
                    infoElement.IsCurrent = true;
                }
            }

            launchLocationsList.AddRange(vacant.Cast<UIElement>().ToList());

            if (vacant.Count > 0 && occupied.Count > 0)
                launchLocationsList.Add(new UIHorizontalSeparator() { Width = new StyleDimension(0, 1), Color = UITheme.Current.SeparatorColor });

            launchLocationsList.AddRange(occupied.Cast<UIElement>().ToList());

            // Add the "Unknown" launch location if no vacant launchpads were found
            if (vacant.Count == 0 && subworldId != MacrocosmSubworld.CurrentID)
            {
                spawnInfoElement = new()
                {
                    FocusContext = "LaunchLocations",
                    IsReachable = true
                };
                spawnInfoElement.OnLeftClick += InfoElement_OnLeftClick;
                spawnInfoElement.OnRightClick += InfoElement_OnRightClick;

                if (Rocket.AtPosition(Utility.SpawnWorldPosition) && subworldId == MacrocosmSubworld.CurrentID)
                    spawnInfoElement.IsCurrent = true;

                launchLocationsList.Add(spawnInfoElement);
            }

            if (current is not null)
            {
                if (launchLocationsList.Any())
                    launchLocationsList.Add(new UIHorizontalSeparator() { Width = new StyleDimension(0, 1), Color = UITheme.Current.SeparatorColor });

                launchLocationsList.Add(current);
            }

            launchLocationsList.Activate();
            return launchLocationsList;
        }


        private void InfoElement_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement is UILaunchDestinationInfoElement infoElement)
            {
                infoElement.HasFocus = true;
            }
        }

        private void InfoElement_OnRightClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement is UILaunchDestinationInfoElement infoElement)
            {
                if (infoElement.HasFocus)
                    infoElement.HasFocus = false;
            }
        }

        private void HandleSeparators()
        {
        }
    }
}
