using Macrocosm.Common.Config;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Dev;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Content.Rockets.Navigation.CrewPanel;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Macrocosm.Content.Rockets.Navigation.NavigationPanel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;

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

        private UIMapTarget lastTarget;
        private UIMapTarget target;

        public UINavigationTab()
        {
            MacrocosmConfig.Instance.OnConfigChanged += (_, _) => ResetInfoPanel();
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

            crewPanel = CreateCrewPanel();
            Append(crewPanel);
            crewPanel.Activate();

            worldInfoPanel = CreateWorldInfoPanel();
            Append(worldInfoPanel);
            worldInfoPanel.Activate();

            flightChecklist = CreateFlightChecklist();
            Append(flightChecklist);
            flightChecklist.Activate();

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
            lastTarget = target;
            target = navigationPanel.CurrentMap.GetSelectedTarget();

			Main.LocalPlayer.RocketPlayer().TargetSubworldID = target is null ? "" : target.Name;

			base.Update(gameTime);

            UpdateInfoPanel();
            UpdateChecklist();
            UpdateLaunchButton();
        }
        private void ResetInfoPanel()
        {
            if (target is not null)
                this.ReplaceChildWith(worldInfoPanel, worldInfoPanel = WorldInfo.ProvideUI(target.Name));
        }

        private void UpdateInfoPanel()
        {
            if (target is not null && target != lastTarget)
                this.ReplaceChildWith(worldInfoPanel, worldInfoPanel = WorldInfo.ProvideUI(target.Name));
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

        private UIListScrollablePanel CreateWorldInfoPanel()
        {
            return WorldInfo.ProvideUI(MacrocosmSubworld.CurrentPlanet);
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
    }
}
