using Macrocosm.Content.Rockets.Navigation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Macrocosm.Content.Rockets
{
    public class RocketUISystem : ModSystem
	{
		public static RocketUISystem Instance => ModContent.GetInstance<RocketUISystem>();
		public UserInterface Interface { get; set; }
		public RocketUIState UIRocketState { get; set; }

		private GameTime lastGameTime;

		public override void Load()
		{
			if (Main.dedServ)
				return;

			Interface = new UserInterface();
		}

		public override void Unload()
		{
			Interface = null;
		}

		public override void OnModLoad()
		{
			
		}

		public override void OnModUnload()
		{
		}

		public override void OnWorldLoad()
		{
			UIRocketState = new RocketUIState();
			UIRocketState.Activate();
		}

		public override void OnWorldUnload()
		{
			UIRocketState = null;
		}  

		public override void OnLocalizationsLoaded()
		{
		}

		public void ShowUI(Rocket rocket)
		{
			if (Main.netMode == NetmodeID.Server && Interface.CurrentState is not null)
				return;  

			Main.playerInventory = true;
			UIRocketState.Rocket = rocket;
			Interface.SetState(UIRocketState);
		}

		public void HideUI()
		{
			if(Main.netMode != NetmodeID.Server)
				Interface?.SetState(null);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			// testing --- press E to reset UI
			if (Main.LocalPlayer.controlHook)
				UIRocketState = new RocketUIState();

			lastGameTime = gameTime;

			if (Interface?.CurrentState != null)
				Interface.Update(gameTime);
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
