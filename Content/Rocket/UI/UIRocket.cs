using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
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

		public static bool Displayed => RocketSystem.Instance.Interface?.CurrentState != null;

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
				!player.InInteractionRange((int)Rocket.Center.X / 16, (int)Rocket.Center.Y / 16) || (Rocket.ModNPC as Rocket).Launching) {
					Hide();
					return;
			}

			UIMapTarget target = NavigationPanel.CurrentMap.GetSelectedTarget();
			player.GetModPlayer<RocketPlayer>().TargetSubworldID = target is null ? "" : target.TargetID;

			string prevName = WorldInfoPanel.Name;
			WorldInfoPanel.Name = target is null ? "" : target.TargetWorldData.DisplayName;
			if (target is not null && prevName != WorldInfoPanel.Name)
 				WorldInfoPanel.FillWithInfo(target.TargetWorldData);
			else if(target is null)
				WorldInfoPanel.FillWithInfo(new Subworlds.SubworldData());

			if (target is null)
 				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.NoTarget;
 			else if(NavigationPanel.CurrentMap.Next != null)
 				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.ZoomIn;
 			else if(!target.CanLaunch())
 				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.CantReach;
 			else
 				LaunchButton.ButtonState = UILaunchButton.ButtonStateType.Launch;
 		}
		
		private void LaunchRocket()
		{
			if(Main.netMode == NetmodeID.MultiplayerClient)
			{
				ModPacket packet = Macrocosm.Instance.GetPacket();
				packet.Write((byte)MessageType.BeginRocketLaunchSequence);
				packet.Write((byte)Main.myPlayer);
				packet.Write((byte)RocketID);
				packet.Send();
			}
			else
			{
				(Rocket.ModNPC as Rocket).Launch(Main.myPlayer);
			}
		}
	}
}
