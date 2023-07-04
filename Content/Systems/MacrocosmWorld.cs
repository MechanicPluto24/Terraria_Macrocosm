using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.Creative;
using SubworldLibrary;
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
 				MacrocosmSubworld.Current.PostUpdateWorld(); 
		}

		private void UpdateTime(MacrocosmSubworld subworld)
		{
			double timeRate = subworld.TimeRate;

			// Fast forward 60 times if using sun/moon-dials
			if (Main.IsFastForwardingTime())
			{
				timeRate *= 60.0;
				Main.desiredWorldTilesUpdateRate = timeRate / 60.0;
				Main.desiredWorldEventsUpdateRate = timeRate;
			}

			// Apply current journey power time modifier
			timeRate *= CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;

			// Apply all players sleeping multiplier 
			if (Main.CurrentFrameFlags.SleepingPlayersCount == Main.CurrentFrameFlags.ActivePlayersCount && Main.CurrentFrameFlags.SleepingPlayersCount > 0)
				timeRate *= 5;

			// Don't pass time if disabled from the journey powers 
			if (CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled)
				timeRate = 0;

			Main.time += timeRate;
			Main.desiredWorldTilesUpdateRate = timeRate / 60.0;
			Main.desiredWorldEventsUpdateRate = timeRate;

			IsDusk = Main.dayTime && Main.time >= subworld.DayLenght;
			IsDawn = !Main.dayTime && Main.time >= subworld.NightLenght;

			if (IsDusk)
			{
				Main.time = 0;
				Main.dayTime = false;

				if(Main.fastForwardTimeToDusk)
					Main.fastForwardTimeToDusk = false;
			}

			if (IsDawn)
			{
				Main.time = 0;
				Main.dayTime = true;

				if (Main.fastForwardTimeToDawn)
					Main.fastForwardTimeToDawn = false;
			}
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

		/// <summary> 
		/// Freezes environment factors like rain or clouds. 
		/// Required when NormalUpdates are turned on (if we ever want that), and as failsafe if something is still updated with them turned off.
		/// </summary>
		private static void FreezeEnvironment()
		{
			//if (Main.gameMenu)
			//	return;

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
	}
}
