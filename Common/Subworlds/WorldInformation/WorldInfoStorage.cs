using System.Collections.Generic;

namespace Macrocosm.Common.Subworlds.WorldInformation
{
    public static class WorldInfoStorage
    {
        public static WorldInfo Sun = new()
        {
            DisplayName = "The Sun",
            Gravity = new(28f, UnitType.Gravity),
            Radius = new(695700f, UnitType.Radius),
            DayPeriod = 0,
            ThreatLevel = 9
        };

        public static WorldInfo Vulcan = new()
        {
            DisplayName = "Vulcan",
            Gravity = new(1f, UnitType.Gravity),
            Radius = new(100f, UnitType.Radius),
            DayPeriod = "Tidally Locked",
            ThreatLevel = 0
        };

        public static WorldInfo Mercury = new()
        {
            DisplayName = "Mercury",
            Gravity = new(0.38f, UnitType.Gravity),
            Radius = new(2439.7f, UnitType.Radius),
            DayPeriod = new(58f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo Venus = new()
        {
            DisplayName = "Venus",
            Gravity = new(0.9f, UnitType.Gravity),
            Radius = new(6051.8f, UnitType.Radius),
            DayPeriod = new(116f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo Mars = new()
        {
            DisplayName = "Mars",
            Gravity = new(0.38f, UnitType.Gravity),
            Radius = new(3389.5f, UnitType.Radius),
            DayPeriod = new(24.62f, UnitType.DayPeriod),
            ThreatLevel = 3
        };

        public static WorldInfo Jupiter = new()
        {
            DisplayName = "Jupiter",
            Gravity = new(2.52f, UnitType.Gravity),
            Radius = new(69911f, UnitType.Radius),
            DayPeriod = new(0.3f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo Saturn = new()
        {
            DisplayName = "Saturn",
            Gravity = new(1.065f, UnitType.Gravity),
            Radius = new(58232f, UnitType.Radius),
            DayPeriod = new(0.43f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo Ouranos = new()
        {
            DisplayName = "Ouranos",
            Gravity = new(0.89f, UnitType.Gravity),
            Radius = new(25362f, UnitType.Radius),
            DayPeriod = new(0.718f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo Neptune = new()
        {
            DisplayName = "Neptune",
            Gravity = new(1.14f, UnitType.Gravity),
            Radius = new(24622f, UnitType.Radius),
            DayPeriod = new(0.671f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo Pluto = new()
        {
            DisplayName = "Pluto",
            Gravity = new(0.064f, UnitType.Gravity),
            Radius = new(1188.3f, UnitType.Radius),
            DayPeriod = new(153.3f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo Eris = new()
        {
            DisplayName = "Eris",
            Gravity = new(0.44f, UnitType.Gravity),
            Radius = new(1163f, UnitType.Radius),
            DayPeriod = new(0f, UnitType.DayPeriod),
            ThreatLevel = 0
        };

        public static WorldInfo InnerSolarSystem = new()
        {
            DisplayName = "    Inner\nSolar System"
        };
    }
}
