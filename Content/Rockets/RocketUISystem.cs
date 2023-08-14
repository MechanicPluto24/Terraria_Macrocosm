using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Navigation;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

		public static void Show(Rocket rocket) => Instance.ShowUI(rocket);
		public static void Hide() => Instance.HideUI();
		public static bool Active => Instance.Interface?.CurrentState is not null;

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
			if (Main.netMode == NetmodeID.Server)
				return;

			UIRocketState = new RocketUIState();
			UIRocketState.Activate();
		}

		public override void OnWorldUnload()
		{
			if (Main.netMode == NetmodeID.Server)
				return;
			
			UIRocketState.Deactivate();
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
			UIRocketState.OnShow();
			Interface.SetState(UIRocketState);
		}

		public void HideUI()
		{
			UIRocketState.OnHide();

			if (Main.netMode != NetmodeID.Server)
				Interface?.SetState(null);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			// press Ctrl + Shift + E to reset UI
			if (Interface.CurrentState is not null &&
				Main.keyState.IsKeyDown(Keys.LeftControl) &&
				Main.keyState.IsKeyDown(Keys.LeftShift) &&
				Main.keyState.IsKeyDown(Keys.E) && !Main.oldKeyState.IsKeyDown(Keys.E))
			{
				Rocket rocket = UIRocketState.Rocket;
				UIRocketState = new RocketUIState();
				UIRocketState.Activate();
				Utility.Chat("Reset rocket UI", Color.Lime);
				HideUI();
				ShowUI(rocket);
			}

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
