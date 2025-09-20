using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Flags;
using Macrocosm.Common.UI.Rockets.Navigation.Checklist;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Achievements;
using Macrocosm.Content.Projectiles.Environment.Meteors;
using Macrocosm.Content.Skies.Ambience.Moon;
using Macrocosm.Content.Skies.Moon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Macrocosm.Content.Subworlds;

/// <summary>
/// Moon terrain and crater generation by 4mbr0s3 2
/// Why isn't anyone else working on this
/// I have saved the day - Ryan
/// </summary>
public partial class Moon : MacrocosmSubworld
{
    public static Moon Instance => ModContent.GetInstance<Moon>();

    // 8 times slower than on Earth (a Terrarian lunar month lasts for 8 in-game days)
    protected override double TimeRate => 0.125;

    // About 6 times lower than default (1, as on Earth)
    protected override float GravityMultiplier(Vector2 position)
    {
        return 0.166f;
    }

    protected override float AtmosphericDensity(Vector2 position) => 0.1f;
    public override int[] EvaporatingLiquidTypes => [LiquidID.Water];
    public override string CustomSky => nameof(MoonSky);

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

    protected override float AmbientTemperature(Vector2 position) => Utility.ScaleNoonToMidnight(-183f, 106f);

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
        meteorStormWaitTimeToStart = Main.rand.Next(62000, 82000);
        meteorStormWaitTimeToEnd = Main.rand.Next(3600, 7200);

        TMLAchievement.Unlock<TravelToMoon>();
    }

    public override void OnExitSubworld()
    {
    }

    public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
    {
        return false;
    }

    public override void PreUpdateEntities()
    {
    }

    private int lastMoonPhase;
    // Macrocosm Moon is set somewhere around Mare Tranquillitatis, which means:
    // - The Moon's "Noon" translates to a Waxing Gibbous 
    // - The Moon's "Midnight" translates to a Waning Crescent 
    private void UpdateMoonPhase()
    {
        int moonPhase;
        if (Main.dayTime)
        {
            // Daytime: 5 phases (20% each of DayLength)
            if (Main.time < DayLength * 0.2)
                moonPhase = (int)MoonPhase.QuarterAtRight; // After Dawn -> Waxing Crescent
            else if (Main.time < DayLength * 0.4)
                moonPhase = (int)MoonPhase.HalfAtRight; // Approaching Noon -> First Quarter
            else if (Main.time < DayLength * 0.6)
                moonPhase = (int)MoonPhase.ThreeQuartersAtRight; // Around Noon -> Waxing Gibbous
            else if (Main.time < DayLength * 0.8)
                moonPhase = (int)MoonPhase.Full; // Approaching Dusk -> Full Moon
            else
                moonPhase = (int)MoonPhase.ThreeQuartersAtLeft; // Before Dusk -> Waning Gibbous
        }
        else
        {
            // Nighttime: 3 phases (33.33% each of NightLength)
            if (Main.time < NightLength * (1.0 / 3.0))
                moonPhase = (int)MoonPhase.HalfAtLeft; // After Dusk -> Third Quarter
            else if (Main.time < NightLength * (2.0 / 3.0))
                moonPhase = (int)MoonPhase.QuarterAtLeft; // Around Midnight -> Waning Crescent
            else
                moonPhase = (int)MoonPhase.Empty; // Approaching Dawn -> New Moon
        }

        if (Main.moonPhase != moonPhase)
        {
            // If the current Moon phase does not match the last expected phase,
            // assume it was set externally and stop forcing it
            if (Main.moonPhase != lastMoonPhase)
            {
                lastMoonPhase = Main.moonPhase;
                SetTimeFromMoonPhase();
            }
            // Otherwise, update the Moon phase to the expected value
            else
            {
                Main.moonPhase = moonPhase;
                lastMoonPhase = moonPhase;

                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
    }

    public void SetTimeFromMoonPhase()
    {
        Main.dayTime = Main.GetMoonPhase()
            is not MoonPhase.HalfAtLeft
            and not MoonPhase.QuarterAtLeft
            and not MoonPhase.Empty;

        // Set time as the middle of the phases
        Main.time = Main.GetMoonPhase() switch
        {
            MoonPhase.QuarterAtRight => DayLength * 0.1,
            MoonPhase.HalfAtRight => DayLength * 0.3,
            MoonPhase.ThreeQuartersAtRight => DayLength * 0.5,
            MoonPhase.Full => DayLength * 0.7,
            MoonPhase.ThreeQuartersAtLeft => DayLength * 0.9,

            MoonPhase.HalfAtLeft => NightLength * (1.0 / 6.0),
            MoonPhase.QuarterAtLeft => NightLength * (3.0 / 6.0),
            MoonPhase.Empty => NightLength * (5.0 / 6.0),

            _ => 0,
        };

        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendData(MessageID.WorldData);
    }

    public override void PreUpdateWorld()
    {
        UpdateMoonPhase();

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
        if (WorldData.DemonSun)
            DemonSunIntensity = 1f;
        else
            DemonSunIntensity = 0f;

        if (DemonSunVisualIntensity < DemonSunIntensity)
            DemonSunVisualIntensity += 0.005f;

        if (DemonSunVisualIntensity > DemonSunIntensity)
            DemonSunVisualIntensity -= 0.005f;
    }

    //TODO 
    private void UpdateSolarStorm() { }
    private void UpdateMeteorStorm()
    {
        meteorStormCounter += Main.worldEventUpdates;

        if (meteorStormWaitTimeToStart <= meteorStormCounter && !WorldData.Current.MeteorStorm)
        {
            Main.NewText(Language.GetTextValue("Mods.Macrocosm.StatusMessages.MeteorStorm.Start"), Color.Gray);
            WorldData.Current.MeteorStorm = true;
            meteorStormCounter = 0;
            meteorStormWaitTimeToStart = Main.rand.Next(62000, 82000);
        }

        if (WorldData.Current.MeteorStorm && meteorStormWaitTimeToEnd <= meteorStormCounter)
        {
            Main.NewText(Language.GetTextValue("Mods.Macrocosm.StatusMessages.MeteorStorm.End"), Color.Gray);
            WorldData.Current.MeteorStorm = false;
            meteorStormCounter = 0;
            meteorStormWaitTimeToEnd = Main.rand.Next(3600, 7200);
        }

        if (WorldData.Current.MeteorStorm)
            MeteorBoost = 300f;
        else
            MeteorBoost = 1f;
    }

    private void UpdateMeteorSpawn()
    {
        meteorTimePass += Main.desiredWorldEventsUpdateRate;
        for (int l = 1; l <= (int)meteorTimePass; l++)
        {
            int closestPlayer;
            int chance = 10000;
            float baseFrequency = 1f;
            float frequency = baseFrequency * MeteorBoost;

            if (Main.rand.Next(chance) < frequency)
            {
                Vector2 position = new((Main.rand.Next(Main.maxTilesX - 50) + 100) * 16, Main.rand.Next((int)(Main.maxTilesY * 0.05)) * 16);

                // 3/4 chance to spawn close to an active (not afk) player on the surface.
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

                    choice.Add(ModContent.ProjectileType<IronMeteorSmall>(), 50.0);
                    choice.Add(ModContent.ProjectileType<IronMeteorMedium>(), 30.0);
                    choice.Add(ModContent.ProjectileType<IronMeteorLarge>(), 12.0);

                    choice.Add(ModContent.ProjectileType<TitaniumMeteorSmall>(), 50.0);
                    choice.Add(ModContent.ProjectileType<TitaniumMeteorMedium>(), 30.0);
                    choice.Add(ModContent.ProjectileType<TitaniumMeteorLarge>(), 12.0);

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
