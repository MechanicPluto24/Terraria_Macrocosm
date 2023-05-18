using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rocket;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
{
	public class UIRocket : UIState
	{
		public int RocketID { get; set; } = -1;

		private NPC Rocket => Main.npc[RocketID];

		UIPanel UIBackgroundPanel;
		UILaunchButton UILaunchButton;
		UINavigationPanel UINavigationPanel;
		UIInfoPanel UIWorldInfoPanel;

		UIFlightChecklist UIFlightChecklist;

		public static void Show(int rocketId) => RocketSystem.Instance.ShowUI(rocketId);
		public static void Hide() => RocketSystem.Instance.HideUI();
		public static bool Active => RocketSystem.Instance.Interface?.CurrentState is not null;

		public override void OnInitialize()
		{
			UIBackgroundPanel = new();
			UIBackgroundPanel.Width.Set(855, 0f);
			UIBackgroundPanel.Height.Set(680, 0f);
			UIBackgroundPanel.HAlign = 0.5f;
			UIBackgroundPanel.VAlign = 0.5f;
			UIBackgroundPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			Append(UIBackgroundPanel);

			UINavigationPanel = new();
			UIBackgroundPanel.Append(UINavigationPanel);

			UIWorldInfoPanel = new("");
			UIBackgroundPanel.Append(UIWorldInfoPanel);

			UIFlightChecklist = new(Language.GetTextValue("Mods.Macrocosm.WorldInfo.Checklist.DisplayName"));
			UIBackgroundPanel.Append(UIFlightChecklist);
			//ChecklistState = new();

			UILaunchButton = new();
			UILaunchButton.ZoomIn = UINavigationPanel.ZoomIn;
			UILaunchButton.Launch = LaunchRocket;
			UIBackgroundPanel.Append(UILaunchButton);
		}

		public override void OnDeactivate()
		{
		}

		private UIMapTarget lastTarget;
		private UIMapTarget target;
		public override void Update(GameTime gameTime)
		{
			// Don't delete this or the UIElements attached to this UIState will cease to function
			base.Update(gameTime);

			Player player = Main.LocalPlayer;
			player.mouseInterface = true;

            if (!Rocket.active || player.dead || !player.active || Main.editChest || Main.editSign || player.talkNPC >= 0 || !Main.playerInventory ||
				!player.InInteractionRange((int)Rocket.Center.X / 16, (int)Rocket.Center.Y / 16, TileReachCheckSettings.Simple) || (Rocket.ModNPC as RocketNPC).Launching || player.controlMount)
			{
				Hide();
				return;
			}

			lastTarget = target;
			target = UINavigationPanel.CurrentMap.GetSelectedTarget();
			player.RocketPlayer().TargetSubworldID = target is null ? "" : target.TargetID;

			GetInfoPanel();
			UpdateChecklist();
			UpdateLaunchButton();
		}

		private void GetInfoPanel()
		{
			if (target is not null && target != lastTarget)
			{
				UIBackgroundPanel.RemoveChild(UIWorldInfoPanel);
				UIWorldInfoPanel = WorldInfoDatabase.GetValue(target.TargetID).ProvideUI();
				UIBackgroundPanel.Append(UIWorldInfoPanel);
			}

			// variant that removes the target on deselection or navigating to the next map
			/*
				WorldInfoPanel.Remove();
				WorldInfoPanel = (target is not null) ? WorldInfoDatabase.GetValue(target.TargetID).ProvideUI() : new UIInfoPanel("");
				BackgroundPanel.Append(WorldInfoPanel);
			*/
		}

		private void UpdateChecklist()
		{
			RocketNPC rocket = Rocket.ModNPC as RocketNPC;

			if (target is null || MacrocosmSubworld.SafeCurrentID == target.TargetID)
			{
				UIFlightChecklist.Destination.State = false;
				UIFlightChecklist.Fuel.State = false;
				UIFlightChecklist.Obstruction.State = false;
			}
			else
			{
				UIFlightChecklist.Destination.State = true;
				UIFlightChecklist.Fuel.State = rocket.Fuel >= RocketFuelLookup.GetFuelCost(MacrocosmSubworld.SafeCurrentID, target.TargetID);
				UIFlightChecklist.Obstruction.State = rocket.CheckFlightPathObstruction();
			}
		}

		private bool CheckCanLaunch()
		{
			if (!target.CanLaunch())
				return false;

			RocketNPC rocket = Rocket.ModNPC as RocketNPC;

			if (rocket.Fuel < RocketFuelLookup.GetFuelCost(MacrocosmSubworld.SafeCurrentID, target.TargetID))
				return false;

			if(!rocket.CheckFlightPathObstruction())
				return false;

			return true;
		}
		private void UpdateLaunchButton()
		{
			if (target is null)
				UILaunchButton.ButtonState = UILaunchButton.StateType.NoTarget;
			else if (UINavigationPanel.CurrentMap.Next != null)
				UILaunchButton.ButtonState = UILaunchButton.StateType.ZoomIn;
			else if (target.AlreadyHere)
				UILaunchButton.ButtonState = UILaunchButton.StateType.AlreadyHere;
			else if (!CheckCanLaunch())
				UILaunchButton.ButtonState = UILaunchButton.StateType.CantReach;
			else
				UILaunchButton.ButtonState = UILaunchButton.StateType.Launch;
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
