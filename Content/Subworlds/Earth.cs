using Macrocosm.Common.Subworlds;
using Terraria;

namespace Macrocosm.Content.Subworlds
{
    /// <summary>
    /// Not really a subworld but could be used for consistency purposes
    /// </summary>
    public static class Earth
    {
		public const double TimeRate = 1.0;

        public static double DayLenght => Main.dayLength;
		public static double NightLenght => Main.nightLength;

        public const float GoreGravity = 0.2f;
        public const float NPCGravity = 0.3f;
        public const float GravityMultiplier = 1f;
		public const float ItemGravity = 0.1f;
		public const float ItemMaxFallSpeed = 7;

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
