using System;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Macrocosm.Content.Tiles;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Macrocosm.Content.Subworlds;
using Macrocosm.Common.Subworlds;

namespace Macrocosm.Content.Systems
{
	class MacrocosmWorld : ModSystem
	{
		public static bool IsDusk = false;
		public static bool IsDawn = false;

		public override void PreUpdateWorld()
		{
			if (MacrocosmSubworld.AnyActive)
			{
				MacrocosmSubworld activeSubworld = MacrocosmSubworld.Current;

				SubworldSystem.hideUnderworld = true;
				SubworldSystem.noReturn = false;

				UpdateTime(activeSubworld);

				activeSubworld.PreUpdateWorld();

				GameMechanicsUpdates();
				FreezeEnvironment();
			}
		}

		public override void PostUpdateWorld()
		{
			if (MacrocosmSubworld.AnyActive)
			{
				MacrocosmSubworld activeSubworld = MacrocosmSubworld.Current;
				activeSubworld.PostUpdateWorld();
			}
		}

		private void UpdateTime(MacrocosmSubworld subworld)
		{
			int timeRateModifier = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
			bool freezeTime = CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled;

			// TODO: add sleeping multiplier 
			Main.time += freezeTime ? 0f : subworld.TimeRate * (Main.IsFastForwardingTime() ? 60 : timeRateModifier) * 34;

			IsDusk = Main.dayTime && Main.time >= subworld.DayLenght;
			IsDawn = !Main.dayTime && Main.time >= subworld.NightLenght;

			if (IsDusk)
			{
				Main.dayTime = false;
				Main.time = 0;
			}

			if (IsDawn)
			{
				Main.dayTime = true;
				Main.time = 0;
			}
		}

		/// <summary>  Freezes environment factors like rain or clouds. Required when NormalUpdates are turned on.</summary>
		private static void FreezeEnvironment()
		{
			if (Main.gameMenu)
				return;

			Main.numClouds = 0;
			Main.windSpeedCurrent = 0;
			Main.weatherCounter = 0;

			// Tricky way to stop vanilla fallen stars for spawning when NormalUpdates are turned on 
			Star.starfallBoost = 0;

			Main.slimeRain = false;
			Main.slimeRainTime = 0;

			Main.StopSlimeRain(false);

			LanternNight.WorldClear();
			Main.StopRain(); // Rain, rain, go away, come again another day
		}

		private static void GameMechanicsUpdates()
		{
			Wiring.UpdateMech();

			TileEntity.UpdateStart();
			foreach (TileEntity te in TileEntity.ByID.Values)
			{
				te.Update();
			}
			TileEntity.UpdateEnd();

			if (++Liquid.skipCount > 1)
			{
				Liquid.UpdateLiquid();
				Liquid.skipCount = 0;
			}
		}
	}
}
