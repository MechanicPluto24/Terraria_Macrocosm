using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content
{
	class MacrocosmWorld : ModSystem
	{
		public static int moonBiome = 0;

		public static int MoonType
		{
			get => moonType;
			set => moonType = Math.Clamp(value, 0, 8);
		}
		private static int moonType = 0;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			moonBiome = tileCounts[ModContent.TileType<Regolith>()];
		}
		public override void ResetNearbyTileEffects()
		{
			moonBiome = 0;
		}

		public override void PreUpdateWorld()
		{
			// Syncs the moon type over the overworld and subworlds 
			if (Main.moonType != MoonType)
				Main.moonType = MoonType;

			// these are needed for subworlds with NormalUpdates turned on 
			if (SubworldSystem.AnyActive<Macrocosm>())
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
		}

		public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
		{
			int timeRateModifier = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
			bool freezeTime = CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled;

			if (SubworldSystem.IsActive<Moon>())
			{
				timeRate = freezeTime ? 0f : Moon.TimeRate * (Main.fastForwardTime ? 60 : timeRateModifier);
			}
		}

		public override void PostWorldGen()
		{
			if (!SubworldSystem.AnyActive<Macrocosm>())
			{
				moonType = Main.moonType;
			}
		}

		public override void LoadWorldData(TagCompound tag)
		{
			tag.TryGet("moonType", out moonType);
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag["moonType"] = moonType;
		}
	}
}
