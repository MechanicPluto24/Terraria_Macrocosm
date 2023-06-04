using Macrocosm.Content.Rocket.Navigation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket
{
    public class RocketSystem : ModSystem
	{
		public static RocketSystem Instance => ModContent.GetInstance<RocketSystem>();
		public UserInterface Interface { get; set; }
		public UINavigation UINavigationState { get; set; }

		private GameTime lastGameTime;

		public override void Load()
		{
			if (Main.dedServ)
				return;

			Interface = new UserInterface();
			UINavigationState = new UINavigation();
			UINavigationState.Activate();
		}

		public override void Unload()
		{
			UINavigationState = null;
		}

		public override void UpdateUI(GameTime gameTime)
		{
			// testing --- press E to reset UI
			if (Main.LocalPlayer.controlHook)
				UINavigationState = new UINavigation();

			lastGameTime = gameTime;
			
			if (Interface?.CurrentState != null)
 				Interface.Update(gameTime);
		}

		public void ShowUI(int rocketId)
		{
			if (Main.netMode == NetmodeID.Server && Interface.CurrentState is not null)
				return;  

			if (rocketId >= 0 && rocketId < Main.maxNPCs) // && (Main.npc[rocketId].ModNPC as Rocket).PlayerID == Main.myPlayer
			{
				Main.playerInventory = true;
				UINavigationState.RocketID = rocketId;
				Interface.SetState(UINavigationState);
			}
		}

		public void HideUI()
		{
			if(Main.netMode != NetmodeID.Server)
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
}
