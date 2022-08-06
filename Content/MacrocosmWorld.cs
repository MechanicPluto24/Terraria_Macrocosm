using Macrocosm.Content.Projectiles;
using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

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

		public static float MeteorBoost { get => meteorBoost; set => meteorBoost = value; }

		private static int moonType = 0;
		private static float meteorBoost = 1f;
		private double timePass = 0.0;

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

			if (SubworldSystem.AnyActive<Macrocosm>())
			{
				Main.numClouds = 0;
				Main.windSpeedCurrent = 0;
				Main.weatherCounter = 0;
				Star.starfallBoost = 0; // Tricky way to stop vanilla fallen stars for spawning with NormalUpdates turned on 

				Main.StopRain(); // Rain, rain, go away, come again another day
				Main.StopSlimeRain();
			}
		}

		public override void PostUpdateWorld()
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				UpdateMeteors();
			}
		}

		public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
		{
			int timeRateModifier = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate * 30;
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
				MoonType = Main.moonType;
			}
		}

		private void UpdateMeteors()
		{

			timePass += Main.desiredWorldEventsUpdateRate;

			for (int l = 1; l <= (int)timePass; l++)
			{

				// Dependent on world size
				float frequency = Main.maxTilesX / 4200f;

				frequency *= MeteorBoost;

				if (!((float)Main.rand.Next(8000) < 10f * frequency))
					continue;

				Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)((double)Main.maxTilesY * 0.05)) * 16);

				// 3/4 chance to spawn close to a player 
				if (!Main.rand.NextBool(4))
				{
					int playerIdx = Player.FindClosest(position, 1, 1);
					if ((double)Main.player[playerIdx].position.Y < Main.worldSurface * 16.0 && Main.player[playerIdx].afkCounter < 3600)
					{
						int offset = Main.rand.Next(1, 640);
						position.X = Main.player[playerIdx].position.X + (float)Main.rand.Next(-offset, offset + 1);
					}
				}

				if (!Collision.SolidCollision(position, 16, 16))
				{
					float speedX = Main.rand.Next(-100, 101);
					float speedY = Main.rand.Next(200) + 100;
					float mult = 12 / (float)Math.Sqrt(speedX * speedX + speedY * speedY);
					speedX *= mult;
					speedY *= mult;

					var source = new EntitySource_Misc("FallingStar");
					Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, ModContent.ProjectileType<FallingMeteor>(), 0, 0f, Main.myPlayer);
				}
			}

			timePass %= 1.0;
		}

	}
}
