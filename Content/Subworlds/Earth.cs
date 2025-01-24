using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.UI.Navigation.Checklist;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Content.Subworlds
{
    /// <summary> Not a subworld, used for fetching the main world defaults of subworld-dependent values </summary>
    public static class Earth
    {
        public const string ID = "Macrocosm/Earth";

        public const double TimeRate = 1.0;
        public const double DayLength = Main.dayLength;
        public const double NightLength = Main.nightLength;

        public const float AtmosphericDensity = 1f;

        public const float GoreGravity = 0.2f;
        public const float NPCGravity = 0.3f;
        public const float GravityMultiplier = 1f;
        public const float ItemGravity = 0.1f;
        public const float ItemMaxFallSpeed = 7;
        public static WorldSize WorldSize { get; set; } = WorldSize.Medium;
        public static ChecklistConditionCollection LaunchConditions { get; } = [];

        public static float AmbientTemperature(Vector2 position)
        {
            SceneData scene = new(position);
            float temperature;

            if (scene.ZoneUnderworldHeight)
            {
                temperature = 70f;
            }
            else if (scene.ZoneSnow)
            {
                if (scene.ZoneRockLayerHeight || scene.ZoneDirtLayerHeight)
                    temperature = -10f;
                else
                    temperature = Utility.ScaleNoonToMidnight(-10, -2);
            }
            else if (scene.ZoneRockLayerHeight || scene.ZoneDirtLayerHeight)
            {
                temperature = 20f;
            }
            else if (scene.ZoneDesert)
            {
                temperature = Utility.ScaleNoonToMidnight(25, 45);
            }
            else if (scene.ZoneJungle)
            {
                temperature = Utility.ScaleNoonToMidnight(30, 40);
            }
            else
            {
                temperature = 20 + Utility.ScaleNoonToMidnight(-5, 5);
            }

            return temperature;
        }
    }
}
