using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class UIRocket : UIState
	{
		public Point16 RocketPosition;

		UIPanel BackgroundPanel;
		UILaunchButton LaunchButton;
		UINavigationPanel NavigationPanel;
		UIWorldInfoPanel WorldInfoPanel;

		public override void OnInitialize()
		{
			BackgroundPanel = new();
			BackgroundPanel.Width.Set(950, 0);
			BackgroundPanel.Height.Set(800, 0);
			BackgroundPanel.HAlign = BackgroundPanel.VAlign = 0.5f;
			Append(BackgroundPanel);

			LaunchButton = new();
			BackgroundPanel.Append(LaunchButton);

			NavigationPanel = new();
			BackgroundPanel.Append(NavigationPanel);

			WorldInfoPanel = new();
			BackgroundPanel.Append(WorldInfoPanel);
		}

		public override void OnDeactivate()
		{
		}

		public override void Update(GameTime gameTime)
		{
			// Don't delete this or the UIElements attached to this UIState will cease to function
			base.Update(gameTime);

			Main.LocalPlayer.mouseInterface = true;

			UIMapTarget target = NavigationPanel.CurrentMap.GetSelectedTarget();
 			WorldInfoPanel.Name = target is null ? "" : target.TargetWorldData.DisplayName;
			WorldInfoPanel.Text = target is null ? "" : target.ParseSubworldData();

			Main.LocalPlayer.GetModPlayer<RocketPlayer>().TargetSubworldID = target is null ? "" : target.TargetID;

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

			Player player = Main.LocalPlayer;
			
			if (player.dead || !player.active || Main.editChest || Main.editSign || player.talkNPC >= 0 || !player.InInteractionRange(RocketPosition.X, RocketPosition.Y) || player.GetModPlayer<RocketPlayer>().InRocket)
				RocketSystem.Instance.HideUI();
		}
		
		private void LaunchButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			ModPacket packet = Macrocosm.Instance.GetPacket();
			packet.Write((byte)MessageType.BeginRocketLaunchSequence);
			packet.Write((byte)Main.myPlayer);
			packet.Write(RocketPosition.X);
			packet.Write(RocketPosition.Y);
			packet.Send();
		}
	}
}
