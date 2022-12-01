using Macrocosm.Content.Rocket.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rocket
{
	public class RocketSystem : ModSystem
	{
		public static RocketSystem Instance => ModContent.GetInstance<RocketSystem>();
		public UserInterface Interface { get; set; }
		public UIRocket State { get; set; }

		private GameTime lastGameTime;

		public override void Load()
		{
			if (Main.dedServ)
				return;

			Interface = new UserInterface();
			State = new UIRocket();
			State.Activate();
		}

		public override void Unload()
		{
			State = null;
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (Main.LocalPlayer.controlHook)
				State = new UIRocket();

			lastGameTime = gameTime;
			if (Interface?.CurrentState != null)
			{
				Interface.Update(gameTime);
			}
		}

		public void ShowUI(Point16 rocketPosition = default)
		{
			State.RocketPosition = rocketPosition;
			Interface.SetState(State);
		}

		public void HideUI()
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
}
