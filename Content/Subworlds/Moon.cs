using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Environment.Meteors;
using Macrocosm.Content.Rockets.UI.Navigation.Checklist;
using Macrocosm.Content.Skies.Ambience.Moon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Content.Subworlds
{
    /// <summary>
    /// Moon terrain and crater generation by 4mbr0s3 2
    /// Why isn't anyone else working on this
    /// I have saved the day - Ryan
    /// </summary>
    public partial class Moon : MacrocosmSubworld
    {
        public static Moon Instance => ModContent.GetInstance<Moon>();

        // 8 times slower than on Earth (a Terrarian lunar month lasts for 8 in-game days)
        public override double TimeRate => 0.125;

        // About 6 times lower than default (1, as on Earth)
        public override float GravityMultiplier => 0.166f;
        public override float AtmosphericDensity => 0.1f;

        public override int[] EvaporatingLiquidTypes => [LiquidID.Water];

        public float DemonSunIntensity { get; set; } = 0f;
        public float DemonSunVisualIntensity { get; set; } = 0f;

        public float MeteorBoost { get; set; } = 1f;

        private double meteorTimePass = 0.0;
        private int meteorStormCounter = 0;
        private int meteorStormWaitTimeToStart;
        private int meteorStormWaitTimeToEnd;

        public override ChecklistConditionCollection LaunchConditions => new()
        {
        };
        public override WorldSize GetSubworldSize(WorldSize earthWorldSize) => WorldSize.Small;

        public override float GetAmbientTemperature()
        {
            float temperature;

            if (Main.time < 0.2f * DayLength)
                temperature = Utility.ScaleNoonToMidnight(-183f, 106f);
            else if (Main.time > 0.8f * Main.dayLength)
                temperature = Utility.ScaleNoonToMidnight(106f, -183f);
            else
                temperature = (Main.time < Main.dayLength / 2) ? -183f : 106f;

            return temperature;
        }

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

        public override void OnEnterSubworld()
        {
            SkyManager.Instance.Activate("Macrocosm:MoonSky");

            meteorStormWaitTimeToStart = Main.rand.Next(62000, 82000);
            meteorStormWaitTimeToEnd = Main.rand.Next(3600, 7200);

            DemonSunIntensity = 0f;
            EventSystem.DemonSun = false;
        }

        public override void OnExitSubworld()
        {
            SkyManager.Instance.Deactivate("Macrocosm:MoonSky");
            DemonSunIntensity = 0f;
        }

        public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
        {
            return false;
        }

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ)
            {
                if (!SkyManager.Instance["Macrocosm:MoonSky"].IsActive())
                    SkyManager.Instance.Activate("Macrocosm:MoonSky");
            }
        }

        public override void PreUpdateWorld()
        {
            UpdateDemonSun();
            UpdateMeteorStorm();
            UpdateSolarStorm();
        }

        public override void PostUpdateWorld()
        {
            UpdateMeteorSpawn();
        }

        private void UpdateDemonSun()
        {
            if (DemonSunVisualIntensity < DemonSunIntensity)
                DemonSunVisualIntensity += 0.005f;

            if (DemonSunVisualIntensity > DemonSunIntensity)
                DemonSunVisualIntensity -= 0.005f;
        }

        //TODO 
        private void UpdateSolarStorm() { }
        private void UpdateMeteorStorm()
        {
            meteorStormCounter++;

            if (meteorStormWaitTimeToStart <= meteorStormCounter && !EventSystem.MoonMeteorStorm)
            {
                Main.NewText(Language.GetTextValue("Mods.Macrocosm.StatusMessages.MeteorStorm.Start"), Color.Gray);
                EventSystem.MoonMeteorStorm = true;
                meteorStormCounter = 0;
                meteorStormWaitTimeToStart = Main.rand.Next(62000, 82000);
            }

            if (EventSystem.MoonMeteorStorm && meteorStormWaitTimeToEnd <= meteorStormCounter)
            {
                Main.NewText(Language.GetTextValue("Mods.Macrocosm.StatusMessages.MeteorStorm.End"), Color.Gray);
                EventSystem.MoonMeteorStorm = false;
                meteorStormCounter = 0;
                meteorStormWaitTimeToEnd = Main.rand.Next(3600, 7200);
            }

            if (EventSystem.MoonMeteorStorm)
                MeteorBoost = 1000f;
            else
                MeteorBoost = 1f;
        }

        private void UpdateMeteorSpawn()
        {
            meteorTimePass += Main.desiredWorldEventsUpdateRate;

            for (int l = 1; l <= (int)meteorTimePass; l++)
            {
                int closestPlayer;
                int chance = 6000;
                float baseFrequency = 1f;
                float frequency = baseFrequency * MeteorBoost;

                if (Main.rand.Next(chance) < frequency)
                {
                    Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)(Main.maxTilesY * 0.05)) * 16);

                    // 3/4 chance to spawn close to a an active (not afk) player on the surface.
                    // In vanilla, this only happens with a 1/15 chance, only in expert mode
                    if (!Main.rand.NextBool(4))
                    {
                        closestPlayer = Player.FindClosest(position, 1, 1);
                        if (Main.player[closestPlayer].position.Y < Main.worldSurface * 16.0 && Main.player[closestPlayer].afkCounter < 3600)
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
                        choice.Add(ProjectileID.FallingStar, 30.0);

                        choice.Add(ModContent.ProjectileType<MoonMeteorSmall>(), 50.0);
                        choice.Add(ModContent.ProjectileType<MoonMeteorMedium>(), 30.0);
                        choice.Add(ModContent.ProjectileType<MoonMeteorLarge>(), 12.0);

                        choice.Add(ModContent.ProjectileType<SolarMeteor>(), 2.0);
                        choice.Add(ModContent.ProjectileType<NebulaMeteor>(), 2.0);
                        choice.Add(ModContent.ProjectileType<StardustMeteor>(), 2.0);
                        choice.Add(ModContent.ProjectileType<VortexMeteor>(), 2.0);


                        int type = choice;
                        var source = type != ProjectileID.FallingStar ? new EntitySource_Misc("Meteor") : new EntitySource_Misc("FallingStar");
                        int damage = 1500;

                        if (type == ProjectileID.FallingStar)
                            damage = 720;
                        else if (type == ModContent.ProjectileType<MoonMeteorSmall>())
                            damage = 500;
                        else if (type == ModContent.ProjectileType<MoonMeteorMedium>())
                            damage = 1000;
                        else if (type == ModContent.ProjectileType<MoonMeteorLarge>())
                            damage = 1500;

                        Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, type, damage, 0f); break;
                    }
                }

                if (Main.rand.Next(chance / 3) < frequency)
                {
                    Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)(Main.maxTilesY * 0.05)) * 16);
                    closestPlayer = Player.FindClosest(position, 1, 1);
                    if (Main.player[closestPlayer].position.Y < Main.worldSurface * 16.0 && Main.player[closestPlayer].afkCounter < 3600)
                    {
                        int offset = Main.rand.Next(60, 640);
                        position.X = Main.player[closestPlayer].position.X + (float)Main.rand.Next(-offset, offset + 1);
                    }
                    MacrocosmAmbientSky.Instance.Spawn<MoonMeteor>(Main.player[closestPlayer], Main.rand.Next());
                }
            }

            meteorTimePass %= 1.0;
        }
    }
}
