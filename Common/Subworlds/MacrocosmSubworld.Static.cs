using Macrocosm.Content.Subworlds;
using SubworldLibrary;

namespace Macrocosm.Common.Subworlds
{
	public enum MapColorType
	{
		SkyUpper,
		SkyLower,
		UndergroundUpper,
		UndergroundLower,
		CavernUpper,
		CavernLower,
		Underworld
	}

	public partial class MacrocosmSubworld 
	{
		/// <summary> Get the current <c>MacrocosmSubworld</c> active instance. 
		/// Earth returns null! You should check for <see cref="AnyActive"/> before accessing this. </summary>
		public static MacrocosmSubworld Current => SubworldSystem.Current as MacrocosmSubworld;

		/// <summary> Whether there is any active subworld belonging to this mod </summary>
		public static bool AnyActive => SubworldSystem.AnyActive<Macrocosm>();

		/// <summary> Whether there is NO active subworld belonging to this mod </summary>
		public static bool EarthActive => !AnyActive;

		// TODO: We could protect the original properties get them only via statics?
		public static double CurrentTimeRate => AnyActive ? Current.TimeRate : Earth.TimeRate;
		public static double CurrentDayLenght => AnyActive ? Current.DayLenght : Earth.DayLenght;
		public static double CurrentNightLenght => AnyActive ? Current.NightLenght : Earth.NightLenght;
		public static float CurrentGravityMultiplier => AnyActive ? Current.GravityMultiplier : Earth.GravityMultiplier;
	}
}
