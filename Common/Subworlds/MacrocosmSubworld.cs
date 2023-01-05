using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SubworldLibrary;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds;

namespace Macrocosm.Common.Subworlds
{
    public abstract partial class MacrocosmSubworld : Subworld, IModType
	{
		/// <summary> Time rate of this subworld, compared to Earth's (1.0) </summary>
 		public virtual double TimeRate { get; set; } = Earth.TimeRate;
		
		/// <summary> Day lenght of this subworld in ticks </summary>
 		public virtual double DayLenght { get; set; } = Earth.DayLenght;
		
		/// <summary> Night lenght of this subworld in ticks </summary>
 		public virtual double NightLenght { get; set; } = Earth.NightLenght;

		/// <summary> The gravity multiplier, measured in G (Earth has 1G) </summary>
 		public virtual float GravityMultiplier { get; set; } = Earth.GravityMultiplier;
		
		
		/// <summary> Pre Update logic for this subworld. Not called on multiplayer clients </summary>
		public virtual void PreUpdateWorld() { }

		/// <summary> Post Update logic for this subworld. Not called on multiplayer clients </summary>
		public virtual void PostUpdateWorld() { }

		/// <summary> Specifies the conditions for reaching this particular subworld </summary>
		public virtual bool CanTravelTo() => true;

		/// <summary> The map background color for each depth layer (Surface, Underground, Cavern, Underworld) </summary>
		public virtual Dictionary<MapColorType, Color> MapColors { get; } = null;

		/// <summary> Flavor information about this subworld, displayed in the navigation UI </summary>
		public virtual WorldInfo WorldInfo { get; }
	}
}
