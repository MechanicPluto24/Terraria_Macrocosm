using Macrocosm.Content.Subworlds.WorldInfomation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Subworlds
{
    /// <summary>
    /// Not really a subworld but could be used for consistency purposes
    /// </summary>
    public static class Earth
    {
        public const float BaseGoreGravity = 0.2f;
        public const float BaseNPCGravity = 0.3f;
        public const float BaseGravityMultiplier = 1f;

        public const double BaseTimeRate = 1.0;

        public static WorldInfo WorldInfo => new()
        {
            DisplayName = "Earth",
            Gravity = new(1f, UnitType.Gravity),
            Radius = new(6371f, UnitType.Radius),
            DayPeriod = new(1, UnitType.DayPeriod),
            ThreatLevel = 1,
            Hazards = new(),
            FlavorText = "Third planet from the Sun, and homeworld of humanity. " +
            "Covered in lush green forests, liquid oceans, and an oxygen-rich atmosphere, " +
            "it is the ideal world for life to comfortably thrive."
        };
    }
}
