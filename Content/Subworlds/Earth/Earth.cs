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
		
		public static SubworldData SubworldData => new()
		{
			DisplayName = "Earth",
			Gravity = 1f,
			Radius = 6371f,
			DayPeriod = 1f,
			ThreatLevel = 1f,
			Hazards = new()
		};
	}
}
