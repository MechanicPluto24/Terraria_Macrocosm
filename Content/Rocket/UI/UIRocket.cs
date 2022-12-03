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
				!player.InInteractionRange((int)Rocket.Center.X / 16, (int)Rocket.Center.Y / 16) || player.GetModPlayer<RocketPlayer>().InRocket)
			{
				Hide();
				return;
			}

			UIMapTarget target = NavigationPanel.CurrentMap.GetSelectedTarget();
			player.GetModPlayer<RocketPlayer>().TargetSubworldID = target is null ? "" : target.TargetID;

 			WorldInfoPanel.Name = target is null ? "" : target.TargetWorldData.DisplayName;
			WorldInfoPanel.Text = target is null ? "" : target.ParseSubworldData();

			if (target is null)
			{
				LaunchButton.TextColor = Color.Gold;
				LaunchButton.Text = "NO TARGET";
				LaunchButton.OnClick -= LaunchButton_OnClick;
			}
			else if(!target.CanLaunch())
			{
				LaunchButton.TextColor = Color.Red;
				LaunchButton.Text = "NOPE";
				LaunchButton.OnClick -= LaunchButton_OnClick;
			}
			else
			{
				LaunchButton.TextColor = new Color(0, 255, 0);
				LaunchButton.Text = "LAUNCH";
				LaunchButton.OnClick += LaunchButton_OnClick;
			}
		}
		
		private void LaunchButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
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
