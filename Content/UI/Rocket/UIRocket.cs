﻿using Macrocosm.Common.Utility;
using Macrocosm.Content.Rocket;
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

			WorldInfoPanel = new();
			BackgroundPanel.Append(WorldInfoPanel);

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

			UIMapTarget target = NavigationPanel.CurrentMap.GetSelectedTarget();
			player.RocketPlayer().TargetSubworldID = target is null ? "" : target.TargetID;

			string prevName = WorldInfoPanel.Name;
			WorldInfoPanel.Name = target is null ? "" : target.TargetWorldInfo.DisplayName;

			if (target is not null && prevName != WorldInfoPanel.Name)
				WorldInfoPanel.FillWithInfo(target.TargetWorldInfo);
			else if (target is null)
				WorldInfoPanel.ClearInfo();

			if (target is null)
				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.NoTarget;
			else if (NavigationPanel.CurrentMap.Next != null)
				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.ZoomIn;
			else if (target.AlreadyHere)
				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.AlreadyHere;
			else if (!target.CanLaunch())
				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.CantReach;
			else
				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.Launch;
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