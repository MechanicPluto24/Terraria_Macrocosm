using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Subworlds
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

	public abstract class MacrocosmSubworld : Subworld, IModType
	{
		/// <summary> Get the current subworld instance. Earth returns null </summary>
		public static MacrocosmSubworld Current() => SubworldSystem.Current as MacrocosmSubworld;

		/// <summary> Returns true if there is any active subworld </summary>
		public static bool AnyActive() => SubworldSystem.AnyActive<Macrocosm>();
		
		/// <summary> Time rate of this subworld, compared to Earth's (1.0) </summary>
		public virtual double TimeRate { get; set; } = 1.0;

		/// <summary> Day lenght of this subworld in ticks </summary>
		public virtual double DayLenght { get; set; } = Main.dayLength;

		/// <summary> Night lenght of this subworld in ticks </summary>
		public virtual double NightLenght { get; set; } = Main.nightLength;

		/// <summary> The gravity multiplier, measured in G (Earth has 1G) </summary>
		public virtual float GravityMultiplier { get; set; } = 1f;

		/// <summary> Pre Update logic for this subworld </summary>
		public virtual void PreUpdateWorld() { }

		/// <summary> Determines the conditions for reaching this particular subworld </summary>
		public virtual bool CanTravelTo() => true;

		/// <summary> Background map color for each depth layer (Surface, Underground, Cavern, Underworld) </summary>
		public virtual Dictionary<MapColorType, Color> MapColors { get; } = null;

		/// <summary> Flavor information about this subworld, displayed in the navigation UI </summary>
		public virtual WorldInfo WorldInfo { get; }
	}
}
