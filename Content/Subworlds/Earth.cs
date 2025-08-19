using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.UI.Navigation.Checklist;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Content.Subworlds;

/// <summary> Not a subworld, used for fetching the main world defaults of subworld-dependent values </summary>
public static class Earth
{
    public const string ID = $"{nameof(Macrocosm)}/{nameof(Earth)}";

    public const double TimeRate = 1.0;
    public const double DayLength = Main.dayLength;
    public const double NightLength = Main.nightLength;

    public const float AtmosphericDensity = 1f;

    public const float GoreGravity = 0.2f;
    public const float NPCGravity = 0.3f;
    public const float GravityMultiplier = 1f;
    public const float ItemGravity = 0.1f;
    public const float ItemMaxFallSpeed = 7;
    public static WorldSize WorldSize => new(MacrocosmSubworld.MainWorldFileData);
    public static ChecklistConditionCollection LaunchConditions { get; } = [];

    /// <summary> The ambient temperature, expressed in °C. Pass position only when you need temperature at a position different than the local player's </summary>
    public static float AmbientTemperature(Vector2? position = null)
    {
        float temperature;

        bool underground;
        bool underworld;
        bool snow;
        bool desert;
        bool jungle;

        if (position.HasValue)
        {
            SceneData scene = new(position.Value);
            underground = scene.ZoneRockLayerHeight || scene.ZoneDirtLayerHeight;
            underworld = scene.ZoneUnderworldHeight;
            snow = scene.ZoneSnow;
            desert = scene.ZoneDesert;
            jungle = scene.ZoneJungle;
        }
        else
        {
            Player player = Main.LocalPlayer;
            underground = player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight;
            underworld = player.ZoneUnderworldHeight;
            snow = player.ZoneSnow;
            desert = player.ZoneDesert;
            jungle = player.ZoneJungle;
        }

        if (underworld)
            temperature = 70f;
        else if (snow)
            temperature = underground ? -10f : Utility.ScaleNoonToMidnight(-10, -2);
        else if (underground)
            temperature = 20f;
        else if (desert)
            temperature = Utility.ScaleNoonToMidnight(25, 45);
        else if (jungle)
            temperature = Utility.ScaleNoonToMidnight(30, 40);
        else
            temperature = 20 + Utility.ScaleNoonToMidnight(-5, 5);

        return temperature;
    }
}
