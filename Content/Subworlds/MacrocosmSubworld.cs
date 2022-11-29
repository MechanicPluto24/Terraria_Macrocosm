using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SubworldLibrary;
using Terraria;
using Terraria.Map;
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

	/// <summary> Stores information about this subworld </summary>
	public struct SubworldData
	{
		public string DisplayName;
		
		public float Gravity;         // in G
		public float Radius;          // in km
		public float DayPeriod;		  // in Earth hours
		public float ThreatLevel;     // scale 1-10 (?)
		public List<string> Hazards;  
	}

	public abstract class MacrocosmSubworld : Subworld
	{
		public static MacrocosmSubworld Current() => SubworldSystem.Current as MacrocosmSubworld;
		public static bool AnyActive() => SubworldSystem.AnyActive<Macrocosm>();

		public virtual double TimeRate { get; set; } = 1.0;
		public virtual double DayLenght { get; set; } = Main.dayLength;
		public virtual double NightLenght { get; set; } = Main.nightLength;
		public virtual float GravityMultiplier { get; set; } = 1f;
		public virtual Dictionary<MapColorType, Color> MapColors { get; } = null;
		public virtual SubworldData SubworldData { get; }
		public virtual bool CanTravelTo() => true;
	}
}
