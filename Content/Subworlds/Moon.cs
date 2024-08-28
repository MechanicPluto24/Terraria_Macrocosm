using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Projectiles.Environment.Meteors;
using Macrocosm.Content.Rockets.UI.Navigation.Checklist;
using Macrocosm.Content.Skies.Ambience.Moon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;
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

        public override ChecklistConditionCollection LaunchConditions => new()
        {
            //new ChecklistCondition("MoonLord", () => NPC.downedMoonlord, hideIfMet: true) // placeholder for now
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
        }

        public override void OnEnterWorld()
        {
            SkyManager.Instance.Activate("Macrocosm:MoonSky");

            meteorStormWaitTimeToStart = Main.rand.Next(62000, 82000);
            meteorStormWaitTimeToEnd = Main.rand.Next(900, 2700);
        }

        public override void OnExitWorld()
        {
            SkyManager.Instance.Deactivate("Macrocosm:MoonSky");
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
            UpdateBloodMoon();
            UpdateMeteorStorm();
            UpdateSolarStorm();
        }

        public override void PostUpdateWorld()
        {
            UpdateMeteorSpawn();
        }

        //TODO
        private void UpdateBloodMoon()
        {
        }

        //TODO 
        private void UpdateSolarStorm() { }

        public bool MeteorStormActive { get; set; } = false;
        public float MeteorBoost { get; set; } = 1f;

        private double meteorTimePass = 0.0;
        private int meteorStormCounter = 0;
        private int meteorStormWaitTimeToStart;
        private int meteorStormWaitTimeToEnd;
        private void UpdateMeteorStorm()
        {
            meteorStormCounter++;

            if (meteorStormWaitTimeToStart <= meteorStormCounter && !MeteorStormActive)
            {
                MeteorStormActive = true;
                meteorStormCounter = 0;
                meteorStormWaitTimeToStart = Main.rand.Next(62000, 82000);
            }

            if (MeteorStormActive && meteorStormWaitTimeToEnd <= meteorStormCounter)
            {
                MeteorStormActive = false;
                meteorStormCounter = 0;
                meteorStormWaitTimeToEnd = Main.rand.Next(900, 2700);
            }

            if (MeteorStormActive)
                MeteorBoost = 1000f;
            else
                MeteorBoost = 1f;
        }

        private void UpdateMeteorSpawn()
        {
            meteorTimePass += Main.desiredWorldEventsUpdateRate;

            int closestPlayer = 0;

            for (int l = 1; l <= (int)meteorTimePass; l++)
            {
                float frequency = 2f * MeteorBoost;

                if (Main.rand.Next(8000) >= frequency)
                    continue;

                Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)((double)Main.maxTilesY * 0.05)) * 16);

                // 3/4 chance to spawn close to a player 
                if (!Main.rand.NextBool(4))
                {
                    closestPlayer = Player.FindClosest(position, 1, 1);
                    if (Main.player[closestPlayer].position.Y < Main.worldSurface * 16.0 && Main.player[closestPlayer].afkCounter < 3600)
                    {
                        int offset = Main.rand.Next(1, 640);
                        position.X = Main.player[closestPlayer].position.X + (float)Main.rand.Next(-offset, offset + 1);
                    }
                }

                for(int i = 0; i < Main.rand.Next(2, 5); i++)
                    MacrocosmAmbientSky.Instance.Spawn<MoonMeteor>(Main.player[closestPlayer], Main.rand.Next());

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

                    var source = Main.player[closestPlayer].GetSource_Misc("Meteor");

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

                    Projectile.NewProjectile(source, position.X, position.Y, speedX, speedY, type, damage, 0f); break;
                }
            }

            meteorTimePass %= 1.0;
        }
    }
}
