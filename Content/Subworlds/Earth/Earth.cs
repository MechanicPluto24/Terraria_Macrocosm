using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Subworlds.Earth
{
	/// <summary>
	/// Not really a subworld but can be used for consistency purposes
	/// </summary>
	public static class Earth
	{
		public static float BaseGravity {get;} = 0.2f;
		
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
