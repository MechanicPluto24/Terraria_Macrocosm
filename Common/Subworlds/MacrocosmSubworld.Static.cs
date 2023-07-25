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

		/// <summary> 
		/// Get the current active Macrocosm subworld string ID, matching the subworld class name. 
		/// Returns <c>Earth</c> if none active. 
		/// Use this for situations where we want other mods subworlds to behave like Earth.
		/// </summary>
		public static string CurrentPlanet => AnyActive ? Current.Name : "Earth";

		/// <summary>
		/// Get the current active subworld string ID, matching the subworld class name. 
		/// If it's from another mod, not Macrocosm, returns the subworld name with the mod name prepended. 
		/// Returns <c>Earth</c> if none active.
		/// Use this for situations where other mods' subworlds will behave differently from Earth (the main world).
		/// </summary>
		public static string CurrentSubworld { 
			get 
			{
				if (AnyActive)
					return Current.Name;
				else if (SubworldSystem.Current is not null)
					return SubworldSystem.Current.Mod.Name + ":" + SubworldSystem.Current.Name;
				else
					return "Earth";
			} 
		}

		/// <summary> Whether there is any active subworld belonging to Macrocosm </summary>
		public static bool AnyActive => SubworldSystem.AnyActive<Macrocosm>();

		// TODO: We could protect the original properties get them only via statics?
		public static double CurrentTimeRate => AnyActive ? Current.TimeRate : Earth.TimeRate;
		public static double CurrentDayLenght => AnyActive ? Current.DayLenght : Earth.DayLenght;
		public static double CurrentNightLenght => AnyActive ? Current.NightLenght : Earth.NightLenght;
		public static float CurrentGravityMultiplier => AnyActive ? Current.GravityMultiplier : Earth.GravityMultiplier;
	}
}
