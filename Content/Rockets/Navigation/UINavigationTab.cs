using Macrocosm.Common.Config;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.Navigation
{
	public class UINavigationTab : UIPanel, ITabUIElement, IRocketDataConsumer
    {
		public Rocket Rocket { get; set; } 

		public UICustomizationPreview CustomizationPreview { get; set; }

        private UILaunchButton launchButton;
        private UINavigationPanel navigationPanel;

		private UIListScrollablePanel worldInfoPanel;
        private UIListScrollablePanel flightChecklist;

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

			navigationPanel = new();
            Append(navigationPanel);

            worldInfoPanel = WorldInfo.ProvideUI(MacrocosmSubworld.CurrentPlanet);
			Append(worldInfoPanel);

            // TODO: move this to a provider class 
            flightChecklist = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Checklist"), scale: 1.2f))
            {
                Top = new(0, 0.365f),
                HAlign = 0.5f,
                Width = new(0, 0.34f),
                Height = new(0, 0.51f),
				BackgroundColor = new(53, 72, 135),
			    BorderColor = new(89, 116, 213, 255)
		    };

			flightChecklist.SetPadding(2f);
			Append(flightChecklist);

            launchButton = new();
            launchButton.ZoomIn = navigationPanel.ZoomIn;
            launchButton.Launch = LaunchRocket;
            Append(launchButton);

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

			lastTarget = target;
            target = navigationPanel.CurrentMap.GetSelectedTarget();
            player.RocketPlayer().TargetSubworldID = target is null ? "" : target.Name;

            UpdateInfoPanel();
            UpdateChecklist();
            UpdateLaunchButton();
        }

		private void UpdateInfoPanel()
        {
            // Update the info panel on new target 
            if (target is not null && (target != lastTarget)) 
 				this.ReplaceChildWith(worldInfoPanel, worldInfoPanel = WorldInfo.ProvideUI(target.Name));
 		}

        private void ResetInfoPanel()
        {
            if(target is not null)
				this.ReplaceChildWith(worldInfoPanel, worldInfoPanel = WorldInfo.ProvideUI(target.Name));
		}

		// TODO: move this to an UI provider class
		// FIXME: list is cleared and populated every tick, should be a better way to do this?
		private void UpdateChecklist()
        {
			flightChecklist.ClearList();

			if (!selectedLaunchCondition.IsMet()) 
				flightChecklist.Add(selectedLaunchCondition.ProvideUI(ChecklistInfoElement.ExtraIconType.QuestionMarkGold));
			// if selected, target is guaranteed to not be null
			else if (!hereLaunchCondition.IsMet()) 
 				flightChecklist.Add(hereLaunchCondition.ProvideUI(ChecklistInfoElement.ExtraIconType.CrossmarkGray));
            else
            {
                if (target.LaunchConditions is not null)
                    flightChecklist.AddRange(ChecklistConditionCollection.Merge(target.LaunchConditions, genericLaunchConditions).ProvideUIElementList());
                else
                    flightChecklist.AddRange(genericLaunchConditions.ProvideUIElementList());
			}

            flightChecklist.Activate();
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
                launchButton.ButtonState = UILaunchButton.StateType.NoTarget;
            else if (navigationPanel.CurrentMap.HasNext) 
                launchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
            else if (target.AlreadyHere)
                launchButton.ButtonState = UILaunchButton.StateType.AlreadyHere;
            else if (!CheckLaunchConditions())
                launchButton.ButtonState = UILaunchButton.StateType.CantReach;
            else
                launchButton.ButtonState = UILaunchButton.StateType.Launch;
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
