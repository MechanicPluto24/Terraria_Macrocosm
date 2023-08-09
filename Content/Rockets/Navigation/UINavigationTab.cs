using Microsoft.Xna.Framework;
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

        public UICustomizationPreview CustomizationPreview;

        private UILaunchButton LaunchButton;
        private UINavigationPanel NavigationPanel;

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

			NavigationPanel = new();
            Append(NavigationPanel);

            WorldInfoPanel = WorldInfoStorage.GetValue(MacrocosmSubworld.CurrentPlanet).ProvideUI();
			Append(WorldInfoPanel);

            // TODO: move this to a provider class 
            FlightChecklist = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.RocketUI.Common.Checklist"), scale: 1.2f))
            {
                Top = new(0, 0.365f),
                HAlign = 0.5f,
                Width = new(0, 0.34f),
                Height = new(0, 0.51f),
				BackgroundColor = new(53, 72, 135),
			    BorderColor = new(89, 116, 213, 255)
		    };

			FlightChecklist.SetPadding(2f);
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
            // Update the info panel on new target 
            if (target is not null && (target != lastTarget)) 
 				this.ReplaceChildWith(WorldInfoPanel, WorldInfoStorage.GetValue(target.Name).ProvideUI());
 		}

        private void ResetInfoPanel()
        {
            if(target is not null)
				this.ReplaceChildWith(WorldInfoPanel, WorldInfoStorage.GetValue(target.Name).ProvideUI());
		}

		// TODO: move this to an UI provider class
		// FIXME: list is cleared and populated every tick, should be a better way to do this?
		private void UpdateChecklist()
        {
			FlightChecklist.ClearList();

			if (!selectedLaunchCondition.IsMet()) 
				FlightChecklist.Add(selectedLaunchCondition.ProvideUI(ChecklistInfoElement.ExtraIconType.GoldQuestionMark));
			// if selected, target is guaranteed to not be null
			else if (!hereLaunchCondition.IsMet()) 
 				FlightChecklist.Add(hereLaunchCondition.ProvideUI(ChecklistInfoElement.ExtraIconType.GrayCrossmark));
            else
            {
                if (target.LaunchConditions is not null)
                    FlightChecklist.AddList(ChecklistConditionCollection.Merge(target.LaunchConditions, genericLaunchConditions).ProvideUIElementList());
                else
                    FlightChecklist.AddList(genericLaunchConditions.ProvideUIElementList());
			}

            FlightChecklist.Activate();
		}

		// TODO: move this to a checker class (same as the UI provider?)
		public bool CheckLaunchConditions()
        {
            bool met = selectedLaunchCondition.IsMet() && hereLaunchCondition.IsMet() && genericLaunchConditions.MetAll();

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

        private float GetFuelCost() => RocketFuelLookup.GetFuelCost(MacrocosmSubworld.CurrentPlanet, target.Name);

        private bool CheckFuel() => Rocket.Fuel >= GetFuelCost();

	}
}
