using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Terraria;

namespace Macrocosm.Content.Subworlds
{
    /// <summary>
    /// Not really a subworld but could be used for code consistency purposes
    /// </summary>
    public static class Earth
    {
        public const double TimeRate = 1.0;
        public const double DayLenght = Main.dayLength;
        public const double NightLenght = Main.nightLength;
        public const float GoreGravity = 0.2f;
        public const float NPCGravity = 0.3f;
        public const float GravityMultiplier = 1f;
        public const float ItemGravity = 0.1f;
        public const float ItemMaxFallSpeed = 7;

        private static WorldSize worldSize = WorldSize.Medium;
        public static WorldSize WorldSize { get => worldSize; set => worldSize = value; }

        public static ChecklistConditionCollection LaunchConditions = new()
        {
        };
    }
}
