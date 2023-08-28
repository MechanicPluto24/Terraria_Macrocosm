using Terraria;
using Macrocosm.Content.UI.LoadingScreens;
using Macrocosm.Content.Rockets.Navigation.Checklist;
using Macrocosm.Common.Subworlds;

namespace Macrocosm.Content.Subworlds
{
    /// <summary>
    /// Not really a subworld but could be used for consistency purposes
    /// </summary>
    internal static class Earth
    {
		public const double TimeRate = 1.0;

        public static double DayLenght => Main.dayLength;
		public static double NightLenght => Main.nightLength;

        public const float GoreGravity = 0.2f;
        public const float NPCGravity = 0.3f;
        public const float GravityMultiplier = 1f;
		public const float ItemGravity = 0.1f;
		public const float ItemMaxFallSpeed = 7;

        private static EarthLoadingScreen loadingScreen;
		public static EarthLoadingScreen LoadingScreen { get { loadingScreen ??= new(); return loadingScreen ;} }

        public static ChecklistConditionCollection LaunchConditions = new()
        {
        };
    }
}
