using Macrocosm.Common.Utils;
using Macrocosm.Content.Rocket;
using Macrocosm.Content.UI.Rocket.WorldInformation;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
{
    public class UIRocket : UIState
	{
		public int RocketID { get; set; } = -1;

		private NPC Rocket => Main.npc[RocketID];

		UIPanel BackgroundPanel;
		UILaunchButton LaunchButton;
		UINavigationPanel NavigationPanel;
		UIWorldInfoPanel WorldInfoPanel;

		public static void Show(int rocketId) => RocketSystem.Instance.ShowUI(rocketId);
		public static void Hide() => RocketSystem.Instance.HideUI();
		public static bool Active => RocketSystem.Instance.Interface?.CurrentState is not null;

		public override void OnInitialize()
		{
			BackgroundPanel = new();
			BackgroundPanel.Width.Set(855, 0f);
			BackgroundPanel.Height.Set(680, 0f);
			BackgroundPanel.HAlign = 0.5f;
			BackgroundPanel.VAlign = 0.5f;
			BackgroundPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			Append(BackgroundPanel);

			NavigationPanel = new();
			BackgroundPanel.Append(NavigationPanel);

			//WorldInfoPanel = new("");
			//BackgroundPanel.Append(WorldInfoPanel);

			LaunchButton = new();
			LaunchButton.ZoomIn = NavigationPanel.ZoomIn;
			LaunchButton.Launch = LaunchRocket;
			BackgroundPanel.Append(LaunchButton);
		}
		public override void OnDeactivate()
		{
		}

		public override void Update(GameTime gameTime)
		{
			// Don't delete this or the UIElements attached to this UIState will cease to function
			base.Update(gameTime);

			Player player = Main.LocalPlayer;
			player.mouseInterface = true;

			if (!Rocket.active || player.dead || !player.active || Main.editChest || Main.editSign || player.talkNPC >= 0 || !Main.playerInventory ||
				!player.InInteractionRange((int)Rocket.Center.X / 16, (int)Rocket.Center.Y / 16) || (Rocket.ModNPC as RocketNPC).Launching || player.controlMount)
			{
				Hide();
				return;
			}

			lastTarget = target;
			target = NavigationPanel.CurrentMap.GetSelectedTarget();
			player.RocketPlayer().TargetSubworldID = target is null ? "" : target.TargetID;

			GenerateInfoPanel(target, lastTarget);
			SetButtonStatus(target);
		}

		private UIMapTarget lastTarget;
		private UIMapTarget target;

		private void GenerateInfoPanel(UIMapTarget target, UIMapTarget lastTarget)
		{
			if (target is not null && target != lastTarget)
			{
				if (WorldInfoPanel is not null)
					BackgroundPanel.RemoveChild(WorldInfoPanel);

				WorldInfoPanel = WorldInfoDatabase.GetValue(target.TargetID).ProvideUI();
				BackgroundPanel.Append(WorldInfoPanel);
			}
		}

		private void SetButtonStatus(UIMapTarget target)
		{
			if (target is null)
				LaunchButton.ButtonState = UILaunchButton.StateType.NoTarget;
			else if (NavigationPanel.CurrentMap.Next != null)
				LaunchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
			else if (target.AlreadyHere)
				LaunchButton.ButtonState = UILaunchButton.StateType.AlreadyHere;
			else if (!target.CanLaunch())
				LaunchButton.ButtonState = UILaunchButton.StateType.CantReach;
			else
				LaunchButton.ButtonState = UILaunchButton.StateType.Launch;
		}

		private void LaunchRocket()
		{
			RocketNPC rocket = Rocket.ModNPC as RocketNPC;
			rocket.Launch(); // launch rocket on the local sp/mp client

			if (Main.netMode == NetmodeID.MultiplayerClient)
				rocket.SendLaunchMessage(); // send launch message to the server
		}
	}
}
