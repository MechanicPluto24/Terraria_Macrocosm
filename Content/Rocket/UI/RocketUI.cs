using Macrocosm.Common.Drawing;
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket.UI
{
	public class RocketUI : ModSystem
	{
		public static UserInterface Interface { get; set; }
		public static RocketUIState State { get; set; }

		private GameTime lastGameTime;

		public override void Load()
		{
			Interface = new UserInterface();
			State = new RocketUIState();
			State.Activate();
		}

		public override void Unload()
		{
			State = null;
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if(Main.LocalPlayer.controlHook)
				State = new RocketUIState();

			lastGameTime = gameTime;
			if (Interface?.CurrentState != null)
			{
				Interface.Update(gameTime);
			}
		}

		public static void Show(Point16 rocketPosition = default)
		{
			State.RocketPosition = rocketPosition;
			Interface.SetState(State);
		}

		public static void Hide()
		{
			Interface?.SetState(null);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Macrocosm:RocketUI",
					() =>
					{
						if (lastGameTime != null && Interface?.CurrentState != null)
							Interface.Draw(Main.spriteBatch, lastGameTime);
						return true;
					},
					InterfaceScaleType.UI));
			}
		}
	}

	public class RocketUIState : UIState
	{
		public Point16 RocketPosition;

		UILaunchButton LaunchButton;
		UINavigationPanel NavigationPanel;
		UIWorldInfoPanel WorldInfoPanel;

		public override void OnInitialize()
		{
			UIPanel panel = new();
			panel.Width.Set(950, 0);
			panel.Height.Set(800, 0);
			panel.HAlign = panel.VAlign = 0.5f;
			Append(panel);

			LaunchButton = new();
			panel.Append(LaunchButton);

			NavigationPanel = new();
			panel.Append(NavigationPanel);

			WorldInfoPanel = new();
			panel.Append(WorldInfoPanel);
		}

		public override void OnDeactivate()
		{
		}

		public override void Update(GameTime gameTime)
		{
			// Don't delete this or the UIElements attached to this UIState will cease to function
			base.Update(gameTime);

			UIMapTarget target = NavigationPanel.CurrentMap.GetSelectedTarget();
 			WorldInfoPanel.Name = target is null ? "" : target.TargetWorldData.DisplayName;
			WorldInfoPanel.Text = target is null ? "" : target.ParseSubworldData();

			Main.LocalPlayer.GetModPlayer<RocketPlayer>().TargetSubworldID = target is null ? "" : target.TargetID;


			if (target is null)
			{
				LaunchButton.TextColor = Color.Gold;
				LaunchButton.Text = "NO TARGET";
				LaunchButton.OnClick += (_,_) => { };
			}
			else if(target.CanLaunch())
			{
				LaunchButton.TextColor = new Color(0, 255, 0);
				LaunchButton.Text = "LAUNCH";
				LaunchButton.OnClick += (_,_) => RocketCommandModuleTile.Launch(target.TargetID, RocketPosition);
			}
			else
			{
				LaunchButton.TextColor = Color.Red;
				LaunchButton.Text = "NOPE";
				LaunchButton.OnClick += (_, _) => { };
			}

			Player player = Main.LocalPlayer;

			if (player.dead || !player.active || Main.editChest || Main.editSign || player.talkNPC >= 0 || !player.InInteractionRange(RocketPosition.X, RocketPosition.Y) || player.GetModPlayer<RocketPlayer>().InRocket)
				RocketUI.Hide();
		}
	}
}
