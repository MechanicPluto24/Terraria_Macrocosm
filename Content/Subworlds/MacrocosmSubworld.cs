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

	/// <summary> Stores information about Macrocosm Subworlds </summary>
	public struct SubworldData
	{		
		public string DisplayName = "";

		public float Gravity = 0f;         // in G
		public float Radius = 0f;          // in km
		public float DayPeriod = 0f;       // in Earth hours
		public float ThreatLevel = 0f;     // scale 1-10 (?)

		public string SpecialGravity = "";
		public string SpecialRadius = "";
		public string SpecialDayPeriod = "";

		public Dictionary<string, string> Hazards = new();

		public SubworldData() { }
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
