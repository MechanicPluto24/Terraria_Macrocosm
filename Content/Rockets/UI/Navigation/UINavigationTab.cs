using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets.LaunchPads;
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

        private UILaunchButton launchButton;
        private UINavigationPanel navigationPanel;

        private UIFlightChecklist flightChecklist;
        private UICommanderPanel commanderPanel;
        private UIListScrollablePanel worldInfoPanel;
        private UIListScrollablePanel launchLocationsList;

        private UINavigationTarget lastTarget;
        private UINavigationTarget target;

        private LaunchPad targetLaunchPad;

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

            launchButton = new();
            launchButton.ZoomIn = navigationPanel.ZoomIn;
            launchButton.Launch = () => Rocket.Launch(target.WorldID, targetLaunchPad);
            Append(launchButton);
            launchButton.Activate();

            CustomizationPreview = new();
            Append(CustomizationPreview);
            CustomizationPreview.Activate();
        }

        public override void OnDeactivate()
        {
        }

        public void OnTabOpen()
        {
            CustomizationPreview.OnTabOpen();
        }

        public override void Update(GameTime gameTime)
        {
            lastTarget = target;
            target = navigationPanel.CurrentMap.GetSelectedTarget();
            Main.LocalPlayer.GetModPlayer<RocketPlayer>().TargetWorld = target is null ? "" : target.WorldID;

            base.Update(gameTime);

            UpdateWorldInfoPanel();
            UpdateLaunchLocationsList();
            UpdateChecklist();
            UpdateLaunchButton();
        }

        private void UpdateWorldInfoPanel()
        {
            if (target is not null && target != lastTarget)
            {
                CreateWorldInfoPanel(target.Name);
                launchLocationsList.ClearList();
            }
        }

        private void UpdateLaunchLocationsList()
        {
            if (target is not null)
            {
                CreateLaunchLocationPanel(target.WorldID);
                launchLocationsList.UpdateOrder();

                if(spawnInfoElement is not null)
                    spawnInfoElement.IsReachable = target is not null && target.IsReachable;

                foreach (var lpInfo in launchLocationsList.OfType<UILaunchDestinationInfoElement>())
                {
                    if (lpInfo.HasFocus)
                    {
                        if (lpInfo.LaunchPad is not null)
                            targetLaunchPad = lpInfo.LaunchPad;
                        else
                            targetLaunchPad = null;

                        break;
                    }
                }
            }
        }

        private void UpdateChecklist()
        {
            flightChecklist.MapTarget = target;
            flightChecklist.TargetLaunchpad = targetLaunchPad;
        }

        private void UpdateLaunchButton()
        {
            if (!flightChecklist.SelectedLaunchCondition.IsMet)
                launchButton.ButtonState = UILaunchButton.StateType.NoTarget;
            else if (navigationPanel.CurrentMap.HasNext)
                launchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
            else if (!flightChecklist.DifferentTargetLaunchCondition.IsMet)
                launchButton.ButtonState = UILaunchButton.StateType.DifferentTarget;
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
                Height = new(0, 0.45f)
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
            subworldId = MacrocosmSubworld.SanitizeID(subworldId);

            if (worldInfoPanel is null)
            {
                worldInfoPanel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.Subworlds." + subworldId + ".DisplayName"), scale: 1.2f))
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

                // Sort by vacant > current > occupied
                launchLocationsList.ManualSortMethod = (list) =>
                {
                    list.Sort((a, b) =>
                    {
                        bool HasRocket(object element) => element is UILaunchDestinationInfoElement infoElement && !infoElement.IsSpawnPointDefault && infoElement.LaunchPad.HasRocket;

                        bool aHasRocket = HasRocket(a);
                        bool bHasRocket = HasRocket(b);

                        if (!aHasRocket && !bHasRocket) return 0;
                        if (!aHasRocket) return -1;
                        if (!bHasRocket) return 1;

                        if (aHasRocket && bHasRocket) return 0;

                        return 0;
                    });
                };
            }

            // Add the launchpads
            foreach (var launchPad in LaunchPadManager.GetLaunchPads(subworldId))
            {
                var storedInfoElement = launchLocationsList.OfType<UILaunchDestinationInfoElement>()
                                                           .FirstOrDefault(e => e.LaunchPad != null && e.LaunchPad == launchPad);

                bool isCurrent = Rocket is not null && launchPad.RocketID == Rocket.WhoAmI;
                bool notFound = storedInfoElement is null;

                // Add any newly created launchpads
                if (notFound)
                {
                    if(!isCurrent)
                    {
                        UILaunchDestinationInfoElement infoElement = new(launchPad)
                        {
                            FocusContext = "LaunchLocations",
                        };
                        infoElement.OnLeftClick += InfoElement_OnLeftClick;

                        launchLocationsList.Add(infoElement);
                    }
                }
                else
                {
                    storedInfoElement.IsCurrent = isCurrent;
                    storedInfoElement.IsReachable = target is not null && target.IsReachable;
                }

            }

            // Remove the current and inactive launchpads
            launchLocationsList.OfType<UILaunchDestinationInfoElement>()
                               .Where(e => !e.IsSpawnPointDefault && (!e.LaunchPad.Active || e.IsCurrent))
                               .ToList()
                               .ForEach(launchLocationsList.RemoveFromList);

            // Add the "Unknown" launch location if no vacant launchpads were found
            if (!launchLocationsList.OfType<UILaunchDestinationInfoElement>().Any(e => e.IsSpawnPointDefault || !e.IsSpawnPointDefault && !e.LaunchPad.HasRocket))
            {
                spawnInfoElement = new()
                {
                    FocusContext = "LaunchLocations",
                };
                spawnInfoElement.OnLeftClick += InfoElement_OnLeftClick;

                if (Rocket.AtPosition(Utility.SpawnWorldPosition) && subworldId == MacrocosmSubworld.CurrentID)
                    spawnInfoElement.IsCurrent = true;

                launchLocationsList.Add(spawnInfoElement);
            }

            launchLocationsList.Activate();
            return launchLocationsList;
        }


        private void InfoElement_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (listeningElement is UILaunchDestinationInfoElement infoElement && infoElement.CanInteract)
                infoElement.HasFocus = true;
        }

        // Doesn't work 
        private void HandleSeparators()
        {
            //Remove all separators 
            launchLocationsList.OfType<UIHorizontalSeparator>()
                               .ToList()
                               .ForEach(launchLocationsList.RemoveFromList);

            // Insert separator after vacant list
            var lastVacantElement = launchLocationsList.OfType<UILaunchDestinationInfoElement>()
                                                       .LastOrDefault(e => e.IsSpawnPointDefault || !e.IsSpawnPointDefault && !e.LaunchPad.HasRocket);

            if (lastVacantElement != null)
            {
                int lastVacantIndex = launchLocationsList.ToList().IndexOf(lastVacantElement);
                bool hasCurrentOrOccupiedAfterLastVacant = launchLocationsList.OfType<UILaunchDestinationInfoElement>()
                                                                              .Any(e => e.IsCurrent || !e.IsSpawnPointDefault && e.LaunchPad.HasRocket);


                if (hasCurrentOrOccupiedAfterLastVacant)
                    launchLocationsList.InsertHorizontalSeparator(lastVacantIndex + 1);
            }


            // Insert separator after the current launchpad
            var currentElement = launchLocationsList.OfType<UILaunchDestinationInfoElement>()
                                                    .FirstOrDefault(e => e.IsCurrent);

            if (currentElement != null)
            {
                int currentIndex = launchLocationsList.ToList().IndexOf(currentElement);
                bool hasOtherOccupiedLaunchPads = launchLocationsList.OfType<UILaunchDestinationInfoElement>()
                                                                     .Any(e => !e.IsCurrent && !e.IsSpawnPointDefault && e.LaunchPad.HasRocket);

                if (hasOtherOccupiedLaunchPads)
                    launchLocationsList.InsertHorizontalSeparator(currentIndex + 1);
            }
        }
    }
}
