﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Common.UI;
using Terraria.Localization;
using Macrocosm.Common.Config;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UINavigationTab : UIPanel
    {
		public Rocket Rocket;

        private UILaunchButton LaunchButton;
        private UINavigationPanel NavigationPanel;

        private UICustomizationPreview CustomizationPreview;

		private UIListScrollablePanel WorldInfoPanel;
        private UIListScrollablePanel FlightChecklist;

        // TODO: move these to a checker (+UI provider?) class
		private ChecklistConditionCollection genericLaunchConditions = new();
        // should these be single-element lists instead?
		private ChecklistCondition selectedLaunchCondition = new("Selected", () => false); 
		private ChecklistCondition hereLaunchCondition = new("NotHere", () => false);

        public UINavigationTab()
        {            
            MacrocosmConfig.Instance.OnConfigChanged += (_,_) => ResetInfoPanel();

			Initialize();
		}

		public override void OnInitialize()
        {
			Width.Set(0, 1f);
			Height.Set(0, 1f);
			HAlign = 0.5f;
			VAlign = 0.5f;

            SetPadding(2f);

            BackgroundColor = new Color(13, 23, 59, 127);
            BorderColor = new Color(15, 15, 15, 255);

			NavigationPanel = new();
            Append(NavigationPanel);

            WorldInfoPanel = WorldInfoStorage.GetValue(MacrocosmSubworld.SafeCurrentID).ProvideUI();
			Append(WorldInfoPanel);

            // TODO: move this to a provider class 
            FlightChecklist = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.RocketUI.Common.Checklist"), scale: 1.2f))
            {
                Top = new StyleDimension(0, 0.365f),
                HAlign = 0.5f,
                Width = new StyleDimension(0, 0.34f),
                Height = new StyleDimension(0, 0.51f),
				BackgroundColor = new Color(53, 72, 135),
			    BorderColor = new Color(89, 116, 213, 255)
		    };
			FlightChecklist.SetPadding(0f);
			Append(FlightChecklist);

            LaunchButton = new();
            LaunchButton.ZoomIn = NavigationPanel.ZoomIn;
            LaunchButton.Launch = LaunchRocket;
            Append(LaunchButton);

			CustomizationPreview = new();
			Append(CustomizationPreview);

			// TODO: move these to a checker (+UI provider?) class
			selectedLaunchCondition = new("Selected", () => target is not null); 
		    hereLaunchCondition = new("NotHere", () => target is not null && !target.AlreadyHere);
		    genericLaunchConditions.Add(new ChecklistCondition("Fuel", CheckFuel));
            genericLaunchConditions.Add(new ChecklistCondition("Obstruction", () => Rocket.CheckFlightPathObstruction()));
		}

		public override void OnDeactivate()
        {
        }

        private UIMapTarget lastTarget;
        private UIMapTarget target;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Player player = Main.LocalPlayer;

            UpdateRocketInstance();

			lastTarget = target;
            target = NavigationPanel.CurrentMap.GetSelectedTarget();
            player.RocketPlayer().TargetSubworldID = target is null ? "" : target.Name;

            UpdateInfoPanel();
            UpdateChecklist();
            UpdateLaunchButton();
        }

        // could have an interface and parent checks for all children and passes the rocket instance?
        public void UpdateRocketInstance()
        {
            CustomizationPreview.Rocket = Rocket;
        }

		private void UpdateInfoPanel()
        {
            // Initial state of the info panel; default to the current subworld
            if (target is null)
            {
				if (lastTarget is not null)
				{
					RemoveChild(WorldInfoPanel);
					WorldInfoPanel = WorldInfoStorage.GetValue(MacrocosmSubworld.SafeCurrentID).ProvideUI();
					Append(WorldInfoPanel);
				}
			}

            // Update the info panel on new target 
            if (target is not null && (target != lastTarget)) 
            {
				RemoveChild(WorldInfoPanel);
				WorldInfoPanel = WorldInfoStorage.GetValue(target.Name).ProvideUI();
				Append(WorldInfoPanel);
			} 
		}

        private void ResetInfoPanel()
        {
            if(target is not null)
            {
			    RemoveChild(WorldInfoPanel);
				WorldInfoPanel = WorldInfoStorage.GetValue(target.Name).ProvideUI();
				Append(WorldInfoPanel);
            }
		}

		// TODO: move this to an UI provider class
		// FIXME: list is cleared and populated every tick, should be a better way to do this?
		private void UpdateChecklist()
        {
			FlightChecklist.ClearList();

			if (!selectedLaunchCondition.Met()) 
				FlightChecklist.Add(selectedLaunchCondition.ProvideUI(ChecklistInfoElement.IconType.QuestionMark));
			// if selected, target is guaranteed to not be null
			else if (!hereLaunchCondition.Met()) 
 				FlightChecklist.Add(hereLaunchCondition.ProvideUI(ChecklistInfoElement.IconType.GrayCrossmark));
            else
            {
                if (target.LaunchConditions is not null)
                    FlightChecklist.AddList(ChecklistConditionCollection.Merge(target.LaunchConditions, genericLaunchConditions).ProvideUIElementList());
                else
                    FlightChecklist.AddList(genericLaunchConditions.ProvideUIElementList());
			}
		}

		// TODO: move this to a checker class (same as the UI provider?)
		public bool CheckLaunchConditions()
        {
            bool met = selectedLaunchCondition.Met() && hereLaunchCondition.Met() && genericLaunchConditions.MetAll();

            if (target is not null)
            {
                met &= target.CheckLaunchConditions();
                target.IsReachable = met;  
            }

            return met;
		}

        private void UpdateLaunchButton()
        {
            if (target is null)
                LaunchButton.ButtonState = UILaunchButton.StateType.NoTarget;
            else if (NavigationPanel.CurrentMap.HasNext) 
                LaunchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
            else if (target.AlreadyHere)
                LaunchButton.ButtonState = UILaunchButton.StateType.AlreadyHere;
            else if (!CheckLaunchConditions())
                LaunchButton.ButtonState = UILaunchButton.StateType.CantReach;
            else
                LaunchButton.ButtonState = UILaunchButton.StateType.Launch;
        }

        private void LaunchRocket()
        {
            Rocket.Fuel -= GetFuelCost();
			Rocket.Launch(); 
        }

        private float GetFuelCost() => RocketFuelLookup.GetFuelCost(MacrocosmSubworld.SafeCurrentID, target.Name);

        private bool CheckFuel() => Rocket.Fuel >= GetFuelCost();

	}
}