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
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Systems
{
	class MacrocosmWorld : ModSystem
	{
		public static bool IsDusk = false;
		public static bool IsDawn = false;

		#region Celestial disco vars
		public enum CelestialType { Nebula, Stardust, Vortex, Solar }
		public static CelestialType CelestialStyle = CelestialType.Nebula;
		public static CelestialType NextCelestialStyle
			=> CelestialStyle == CelestialType.Solar ? CelestialType.Nebula : CelestialStyle + 1;

		public static float CelestialStylePercent;
		private static int celesialCounter = 0;

 		public static Color NebulaColor = new(165, 0, 204);
		public static Color StardustColor = new(0, 187, 255);
		public static Color VortexColor = new(0, 255, 180);
		public static Color SolarColor = new(255, 191, 0);

		public static Color CelestialColor;
		private static readonly Color[] celestialColors = { NebulaColor, StardustColor, VortexColor, SolarColor };
		#endregion

		public override void PreUpdateWorld()
		{
			if (SubworldSystem.AnyActive<Macrocosm>())
			{
				MacrocosmSubworld activeSubworld = SubworldSystem.Current as MacrocosmSubworld;

				SubworldSystem.hideUnderworld = true;
				SubworldSystem.noReturn = false;

				UpdateTime(activeSubworld);

				activeSubworld.PreUpdateWorld();

				GameMechanicsUpdates();
				FreezeEnvironment();
			}
		}

		public override void PostUpdateEverything()
		{
			UpdateCelestialStyle();
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

		/// <summary>
		/// These are needed for subworlds with NormalUpdates turned on 
		/// </summary>
		private static void FreezeEnvironment()
		{
			if (Main.gameMenu)
				return;

			Main.numClouds = 0;
			Main.windSpeedCurrent = 0;
			Main.weatherCounter = 0;
			Star.starfallBoost = 0; // Tricky way to stop vanilla fallen stars for spawning with NormalUpdates turned on 

			Main.slimeRain = false;
			Main.slimeRainTime = 0;

			//if(SkyManager.Instance["Slime"] is not null)
			//	SkyManager.Instance["Slime"].Deactivate();

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

		private static void UpdateCelestialStyle()
		{
			float cyclePeriod = 90f;
			if (celesialCounter++ >= (int)cyclePeriod)
			{
				celesialCounter = 0;
				CelestialStyle = NextCelestialStyle;
			}

			CelestialStylePercent = celesialCounter / cyclePeriod;

			CelestialColor = Color.Lerp(celestialColors[(int)CelestialStyle], celestialColors[(int)NextCelestialStyle], CelestialStylePercent);
		}
	}
}
