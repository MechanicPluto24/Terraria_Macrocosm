using Macrocosm.Common.Config;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Dev;
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
        private ChecklistInfoProvider flightChecklist;
        private UICrewPanel crewPanel;

        private UIListScrollablePanel worldInfoPanel;

		private UIMapTarget lastTarget;
		private UIMapTarget target;

		public UINavigationTab()
        {            
            MacrocosmConfig.Instance.OnConfigChanged += (_,_) => ResetInfoPanel();
		}

        UIListScrollablePanel temp;

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

			crewPanel = CreateCrewPanel();
            Append(crewPanel);
            crewPanel.Activate();

            temp = new UIListScrollablePanel();
			temp.Width.Set(0, 0.34f);
			temp.Height.Set(0f, 0.4f);
			temp.HAlign = 0.5f;
			temp.Top.Set(0f, 0.365f);
			temp.SetPadding(0f);
			temp.BorderColor = new Color(89, 116, 213, 255);
			temp.BackgroundColor = new Color(53, 72, 135);

			Append(temp);

            worldInfoPanel = CreateWorldInfoPanel();
			Append(worldInfoPanel);
            worldInfoPanel.Activate();

            flightChecklist = new();
			Append(CreateFlightChecklist());

            launchButton = new();
            launchButton.ZoomIn = navigationPanel.ZoomIn;
            launchButton.Launch = () => Rocket.Launch(target.Name);
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
            base.Update(gameTime);

			lastTarget = target;
            target = navigationPanel.CurrentMap.GetSelectedTarget();

            UpdateInfoPanel();
            UpdateChecklist();
            UpdateLaunchButton();

            if (temp is null)
                return;

            temp.Deactivate();
            temp.ClearList();

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				var player = Main.player[i];

				if (!player.active)
					continue;

				var rocketPlayer = player.GetModPlayer<RocketPlayer>();

				if (rocketPlayer.InRocket && rocketPlayer.RocketID == Rocket.WhoAmI)
				{
                    temp.Add(new UIInfoElement(new LocalizedColorScaleText(Language.GetText(player.name)))
                    {
                        Width = new(0f, 1f),
                        ExtraDraw = (Vector2 iconPosition) => Main.PlayerRenderer.DrawPlayerHead(Main.Camera, player, Utility.ScreenCenter)
                    }); 
				}
			}

            temp.Activate();


		}

		private void UpdateInfoPanel()
        {
            if (target is not null && (target != lastTarget)) 
 				this.ReplaceChildWith(worldInfoPanel, worldInfoPanel = WorldInfo.ProvideUI(target.Name));
 		}

        private void ResetInfoPanel()
        {
            if(target is not null)
				this.ReplaceChildWith(worldInfoPanel, worldInfoPanel = WorldInfo.ProvideUI(target.Name));
		}

		private void UpdateChecklist()
        {
            flightChecklist.Update();
            flightChecklist.MapTarget = target;

			// FIXME: it's not an UI element so automatic assignment doesn't work :(
            // TODO: it could be an UI element only class instead 
			flightChecklist.Rocket = Rocket;
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
            else if(Main.LocalPlayer.GetModPlayer<RocketPlayer>().AsCommander)
                launchButton.ButtonState = UILaunchButton.StateType.Launch;
            else
				launchButton.ButtonState = UILaunchButton.StateType.LaunchInactive;
		}

        private UIListScrollablePanel CreateWorldInfoPanel()
        {
			return WorldInfo.ProvideUI(MacrocosmSubworld.CurrentPlanet);
		}

        private UIListScrollablePanel CreateFlightChecklist()
        {
			var flightChecklistPanel = flightChecklist.ProvideUI();
			flightChecklistPanel.SetPadding(2f);
            flightChecklistPanel.Top = new(0, 0.365f);
            flightChecklistPanel.Left = new(0, 0.68f);

            flightChecklistPanel.Height = new(0, 0.51f);

            return flightChecklistPanel;
        }

        private UICrewPanel CreateCrewPanel()
        {
            return new UICrewPanel();
        }

	}
}
