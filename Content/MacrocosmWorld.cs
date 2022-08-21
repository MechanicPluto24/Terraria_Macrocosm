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
using Macrocosm.Content.Subworlds;
using Macrocosm.Content.Subworlds.Moon;
using Terraria.ID;

namespace Macrocosm.Content
{
	class MacrocosmWorld : ModSystem
	{
		public static int MoonType
		{
			get => moonType;
			set => moonType = Math.Clamp(value, 0, 8);
		}
		private static int moonType = 0;

		public static bool IsDusk = false; 
		public static bool IsDawn = false; 

		public override void PreUpdateWorld()
		{
			// Syncs the moon type over the overworld and subworlds 
			if (Main.moonType != MoonType)
				Main.moonType = MoonType;

			if (SubworldSystem.AnyActive<Macrocosm>())
			{
				MacrocosmSubworld activeSubworld = (MacrocosmSubworld)SubworldSystem.Current;

				UpdateTime(activeSubworld);
				CustomUpdates(activeSubworld);
				GameMechanicsUpdates();
				FreezeEnvironment();
			}
		}

		public override void PostWorldGen()
		{
			if (!SubworldSystem.AnyActive<Macrocosm>())
				moonType = Main.moonType;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			tag.TryGet("moonType", out moonType);
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["moonType"] = moonType;
		}

		private void UpdateTime(MacrocosmSubworld subworld)
		{
			int timeRateModifier = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
			bool freezeTime = CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled;

			// TODO: add sleeping multiplier 
			Main.time += freezeTime ? 0f : subworld.TimeRate * (Main.fastForwardTime ? 60 : timeRateModifier);

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

		private void CustomUpdates(MacrocosmSubworld subworld)
		{
			if(subworld is Moon)
			{
				// handled by server 
				if(Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (IsDusk && Main.rand.NextBool(9))
						Main.bloodMoon = true;

					if(IsDawn && Main.bloodMoon)
						Main.bloodMoon = false;
				}
			}
			// else if(subworld is Mars) ...
		}

		/// <summary>
		/// These are needed for subworlds with NormalUpdates turned on 
		/// </summary>
		private void FreezeEnvironment()
		{
			Main.numClouds = 0;
			Main.windSpeedCurrent = 0;
			Main.weatherCounter = 0;
			Star.starfallBoost = 0; // Tricky way to stop vanilla fallen stars for spawning with NormalUpdates turned on 

			Main.slimeRain = false;
			Main.slimeRainTime = 0;
			SkyManager.Instance["Slime"].Deactivate();
			Main.StopSlimeRain(false);

			LanternNight.WorldClear();
			Main.StopRain(); // Rain, rain, go away, come again another day
		}

		private void GameMechanicsUpdates()
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
