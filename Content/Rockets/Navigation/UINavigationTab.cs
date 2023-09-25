using Macrocosm.Common.Config;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Dev;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Content.Rockets.Navigation.CrewPanel;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Macrocosm.Content.Rockets.Navigation.NavigationPanel;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UINavigationTab : UIPanel, ITabUIElement, IRocketDataConsumer
    {
        public Rocket Rocket { get; set; }

        public UICustomizationPreview CustomizationPreview { get; set; }

        private UILaunchButton launchButton;
        private UINavigationPanel navigationPanel;

        private UIFlightChecklist flightChecklist;
        private UICrewPanel crewPanel;
        private UIListScrollablePanel worldInfoPanel;
        private UIListScrollablePanel launchLocationPanel;

        private UIMapTarget lastTarget;
        private UIMapTarget target;

        private LaunchPad targetLaunchPad;

        public UINavigationTab()
        {
            MacrocosmConfig.Instance.OnConfigChanged += (_, _) => 
            {
                if (target is not null)
					CreateWorldInfoPanel(target.Name);
			};
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            SetPadding(3f);

            BackgroundColor = new Color(13, 23, 59, 127);
            BorderColor = new Color(15, 15, 15, 255);

            navigationPanel = new();
            Append(navigationPanel);
            navigationPanel.Activate();

            flightChecklist = CreateFlightChecklist();
            Append(flightChecklist);
            flightChecklist.Activate();

            crewPanel = CreateCrewPanel();
            Append(crewPanel);
            crewPanel.Activate();

            worldInfoPanel = CreateWorldInfoPanel(MacrocosmSubworld.CurrentPlanet);
            Append(worldInfoPanel);
            worldInfoPanel.Activate();

            launchLocationPanel = CreateLaunchLocationPanel(MacrocosmSubworld.CurrentPlanet);
            Append(launchLocationPanel);
            launchLocationPanel.Activate();

			launchButton = new();
            launchButton.ZoomIn = navigationPanel.ZoomIn;
            launchButton.Launch = () => Rocket.Launch(target.Name, targetLaunchPad);
            Append(launchButton);
            launchButton.Activate();

            CustomizationPreview = new();
            Append(CustomizationPreview);
            CustomizationPreview.Activate();
        }

        public override void OnDeactivate()
        {
        }

        public override void Update(GameTime gameTime)
        {
            lastTarget = target;
            target = navigationPanel.CurrentMap.GetSelectedTarget();

			Main.LocalPlayer.RocketPlayer().TargetSubworldID = target is null ? "" : target.Name;

			base.Update(gameTime);

			if (target is not null && target != lastTarget)
            {
				CreateWorldInfoPanel(target.Name);
                launchLocationPanel.ClearList();
			}

			if (target is not null)
            {
			    CreateLaunchLocationPanel(target.Name);
                launchLocationPanel.UpdateOrder();
			}

			UpdateChecklist();
            UpdateLaunchButton();
        }

		private void UpdateChecklist()
        {
            flightChecklist.MapTarget = target;
        }

        private void UpdateLaunchButton()
        {
            if (target is null)
                launchButton.ButtonState = UILaunchButton.StateType.NoTarget;
            else if (navigationPanel.CurrentMap.HasNext)
                launchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
            else if (target.AlreadyHere)
                launchButton.ButtonState = UILaunchButton.StateType.AlreadyHere;
            else if (!flightChecklist.CheckLaunchConditions())
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

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                flightChecklist.Height.Percent = 0.28f;
            }

			return flightChecklist;
        }

        private UICrewPanel CreateCrewPanel()
        {
            crewPanel = new UICrewPanel
            {
                Top = new(0f, 0.835f),
                Width = new(0f, 0.31f),
                Left = new(0, 0.68f),
                Height = new(0f, 0.15f),
		    };
            crewPanel.SetPadding(2f);

			if (Main.netMode == NetmodeID.MultiplayerClient)
            {
				crewPanel.Height.Percent = 0.322f;
				crewPanel.Top.Percent = 0.66f;
            }

			return crewPanel;
        }

		private UIListScrollablePanel CreateWorldInfoPanel(string subworldId)
		{
            if(worldInfoPanel is null)
            {
				worldInfoPanel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.Subworlds." + subworldId + ".DisplayName"), scale: 1.2f))
				{
					Width = new(0, 0.31f),
					Height = new(0, 0.62f),
					Left = new(0, 0.01f),
					Top = new(0, 0.365f),
					BackgroundColor = new Color(53, 72, 135),
					BorderColor = new Color(89, 116, 213, 255)
				};
				worldInfoPanel.SetPadding(0f);
			}
            else
            {
				worldInfoPanel.Deactivate();
				worldInfoPanel.ClearList();
                worldInfoPanel.SetTitle(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.Subworlds." + subworldId + ".DisplayName"), scale: 1.2f));
			}
			
			LocalizedText flavorText = Utility.GetLocalizedTextOrEmpty("Mods.Macrocosm.Subworlds." + subworldId + ".FlavorText");
			if (flavorText != LocalizedText.Empty && flavorText.Value != "default")
			{
				worldInfoPanel.Add(new UIDynamicTextPanel(new LocalizedColorScaleText(flavorText, Color.White, scale: 0.85f)));
				worldInfoPanel.AddSeparator();
			}

			List<InfoElement> elements = WorldInfo.GetInfoElements(subworldId);

			if (elements is not null)
			{
				bool foundHazards = false;
				foreach (InfoElement element in elements)
				{
					if (!foundHazards && element is HazardInfoElement)
					{
						worldInfoPanel.AddSeparator();
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
            if(launchLocationPanel is null)
            {
				launchLocationPanel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.LaunchLocations"), scale: 1.2f))
				{
					Top = new(0, 0.365f),
					Width = new(0f, 0.34f),
					HAlign = 0.5f,
					Height = new(0, 0.505f),
					BorderColor = new Color(89, 116, 213, 255),
					BackgroundColor = new Color(53, 72, 135)
				};
				launchLocationPanel.SetPadding(0f);

                launchLocationPanel.ManualSortMethod = (list) =>
                {
                    var selected = list.Where(element => element is UILaunchPadInfoElement infoElement && infoElement.LaunchPad.HasRocket);
                    list = list.Except(selected).ToList();
                    list.AddRange(selected);
				};
			}

			foreach (var launchPad in LaunchPadManager.GetLaunchPads(subworldId))
            {
                var storedInfoElement = launchLocationPanel.OfType<UILaunchPadInfoElement>().FirstOrDefault(lpInfo => lpInfo.LaunchPad == launchPad);
                bool notFound = storedInfoElement is null;

				if (notFound)
                {
					UILaunchPadInfoElement infoElement = new(launchPad)
					{
						FocusContext = "LaunchLocations",
					};
					infoElement.OnLeftClick += InfoElement_OnLeftClick;

                    if (Rocket is not null && launchPad.RocketID == Rocket.WhoAmI)
                        infoElement.IsCurrent = true;

                    launchLocationPanel.Add(infoElement);
  				}
			}

            //launchLocationPanel.OfType<UILaunchPadInfoElement>().Where(infoElement => !infoElement.LaunchPad.Active).ToList().ForEach(RemoveChild);

            launchLocationPanel.Activate();
			return launchLocationPanel;
		}

		private void InfoElement_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
            if(listeningElement is UILaunchPadInfoElement infoElement)
            {
                if (infoElement.LaunchPad.HasRocket)
                    return;

				infoElement.HasFocus = true;
				targetLaunchPad = infoElement.LaunchPad;
			}
		}
	}
}
