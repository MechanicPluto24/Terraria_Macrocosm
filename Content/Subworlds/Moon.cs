using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Sky;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI.Rockets.Navigation.Checklist;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Achievements;
using Macrocosm.Content.Skies.Moon;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Macrocosm.Content.Liquids;
using ModLiquidLib.ModLoader;

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
    public override bool SupportsMeteorStorms => true;
    public override int[] EvaporatingLiquidTypes => [LiquidID.Water, LiquidID.Honey, LiquidLoader.LiquidType<Oil>(), LiquidLoader.LiquidType<RocketFuel>()];
    public override string CustomSky => nameof(MoonSky);

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
        ModContent.GetInstance<TravelToMoon>()?.Condition?.Complete();
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
                moonPhase = (int)MoonPhase.QuarterAtRight;
            else if (Main.time < DayLength * 0.4)
                moonPhase = (int)MoonPhase.HalfAtRight;
            else if (Main.time < DayLength * 0.6)
                moonPhase = (int)MoonPhase.ThreeQuartersAtRight;
            else if (Main.time < DayLength * 0.8)
                moonPhase = (int)MoonPhase.Full;
            else
                moonPhase = (int)MoonPhase.ThreeQuartersAtLeft;
        }
        else
        {
            // Nighttime: 3 phases (33.33% each of NightLength)
            if (Main.time < NightLength * (1.0 / 3.0))
                moonPhase = (int)MoonPhase.HalfAtLeft;
            else if (Main.time < NightLength * (2.0 / 3.0))
                moonPhase = (int)MoonPhase.QuarterAtLeft;
            else
                moonPhase = (int)MoonPhase.Empty;
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
    }
}
