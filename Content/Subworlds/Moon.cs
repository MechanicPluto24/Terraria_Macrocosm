using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using Terraria.Graphics.Effects;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Systems;
using Macrocosm.Content.UI.LoadingScreens;
using Macrocosm.Content.WorldGeneration.Moon;
using Macrocosm.Content.Projectiles.Environment.Meteors;
using Macrocosm.Content.Rocket.Navigation.LaunchConds;

namespace Macrocosm.Content.Subworlds
{
    /// <summary>
    /// Moon terrain and crater generation by 4mbr0s3 2
    /// Why isn't anyone else working on this
    /// I have saved the day - Ryan
    /// </summary>
    public class Moon : MacrocosmSubworld
    {
        private static Moon instance;
        public static Moon Instance { get { instance ??= new(); return instance; } }

        /// <summary> 8 times slower than on Earth (a Terrarian lunar month lasts for 8 in-game days) </summary>
        public override double TimeRate => 0.125;

		/// <summary> About 6 times lower than default (1, as on Earth) </summary>
		public override float GravityMultiplier => 0.166f;

        public override bool NormalUpdates => false;
        public override int Width => 4200;
        public override int Height => 1200;
        public override bool ShouldSave => true;
        public override bool NoPlayerSaving => false;

		private GroundPass genGroundPass;
		public override List<GenPass> Tasks => new()
        {
            genGroundPass,
            new CraterPass("CraterPass", 1f),
             new RegolithPass("RegolithPass", 5f),
            new OrePass("OrePass", 0.75f),
            new CavePass("CavePass", 1f, genGroundPass.RockLayerHigh, genGroundPass.RockLayerHigh),
            new IrradiationPass("IrradiationPass", 3f),
            new ScuffedSmoothPass("ScuffedSmoothPass", 1f),
            new AmbientPass("AmbientPass", 0.2f),
            new FinishPass("FinishPass", 0.1f)
        };

		public override LaunchConditions LaunchConditions => new()
		{
			new LaunchCondition("MoonLord", () => NPC.downedMoonlord)
		};

        public override Dictionary<MapColorType, Color> MapColors => new()
        {
            {MapColorType.SkyUpper, new Color(10, 10, 10)},
            {MapColorType.SkyLower, new Color(40, 40, 40)},
            {MapColorType.UndergroundUpper, new Color(40, 40, 40)},
            {MapColorType.UndergroundLower, new Color(30, 30, 30)},
            {MapColorType.CavernUpper, new Color(30, 30, 30)},
            {MapColorType.CavernLower, new Color(30, 30, 30)},
            {MapColorType.Underworld,  new Color(30, 30, 30)}
        };

        public Moon()
        {
            LoadingScreen = new MoonLoadingScreen();

            genGroundPass = new GroundPass("GroundPass", 8f, Width, Height);
        }

		public override void OnEnterWorld()
        {
            SkyManager.Instance.Activate("Macrocosm:MoonSky");
        }

        public override void OnExitWorld()
        {
            SkyManager.Instance.Deactivate("Macrocosm:MoonSky");
        }

        public override void PreUpdateWorld()
        {
            UpdateBloodMoon();
            UpdateMeteorStorm();
            UpdateSolarStorm();
		}

		public override void PostUpdateWorld()
		{
			UpdateMeteors();
		}

		#region Moon events

        public void UpdateBloodMoon()
        {
			if (MacrocosmWorld.IsDusk && Main.rand.NextBool(9))
				Main.bloodMoon = true;

			if (MacrocosmWorld.IsDawn && Main.bloodMoon)
				Main.bloodMoon = false;
		}

        /// <summary> TODO </summary>
        public void UpdateMeteorStorm() { }

		/// <summary> TODO </summary>
		public void UpdateSolarStorm() { }

		#endregion

		#region Meteor logic

		public static float MeteorBoost = 1f;
		private double timePass = 0.0;
		private void UpdateMeteors()
		{
			// handled by server 
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			timePass += Main.desiredWorldEventsUpdateRate;

			int closestPlayer = 0;

			for (int l = 1; l <= (int)timePass; l++)
			{
				// Dependent on world size
				float frequency = Main.maxTilesX / 4200f;

				frequency *= MeteorBoost;

				if (!((float)Main.rand.Next(8000) < frequency))
					continue;

				Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)((double)Main.maxTilesY * 0.05)) * 16);

				// 3/4 chance to spawn close to a player 
				if (!Main.rand.NextBool(4))
				{
					closestPlayer = Player.FindClosest(position, 1, 1);
					if ((double)Main.player[closestPlayer].position.Y < Main.worldSurface * 16.0 && Main.player[closestPlayer].afkCounter < 3600)
					{
						int offset = Main.rand.Next(1, 640);
						position.X = Main.player[closestPlayer].position.X + (float)Main.rand.Next(-offset, offset + 1);
					}
				}

				if (!Collision.SolidCollision(position, 16, 16))
				{
					float speedX = Main.rand.Next(-100, 101);
					float speedY = Main.rand.Next(200) + 100;
					float mult = 8 / (float)Math.Sqrt(speedX * speedX + speedY * speedY);
					speedX *= mult;
					speedY *= mult;

					WeightedRandom<int> choice = new(Main.rand);
					choice.Add(ModContent.ProjectileType<MoonMeteorSmall>(), 50.0);
					choice.Add(ModContent.ProjectileType<MoonMeteorMedium>(), 33.0);
					choice.Add(ModContent.ProjectileType<MoonMeteorLarge>(), 12.0);

					choice.Add(ModContent.ProjectileType<SolarMeteor>(), 2.0);
					choice.Add(ModContent.ProjectileType<NebulaMeteor>(), 2.0);
					choice.Add(ModContent.ProjectileType<StardustMeteor>(), 2.0);
					choice.Add(ModContent.ProjectileType<VortexMeteor>(), 2.0);

					var source = Main.player[closestPlayer].GetSource_Misc("FallingStar");

					int type = choice;
					int damage;

					if (type == ModContent.ProjectileType<MoonMeteorSmall>())
						damage = 500;
					else if (type == ModContent.ProjectileType<MoonMeteorMedium>())
						damage = 1000;
					else if (type == ModContent.ProjectileType<MoonMeteorLarge>())
						damage = 1500;
					else
						damage = 2000;

					Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, type, damage, 0f, 255); break;

				}
			}

			timePass %= 1.0;
		}

		#endregion

	}
}
