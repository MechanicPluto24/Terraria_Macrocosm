using System.Collections.Generic;

namespace Macrocosm.Content.Subworlds
{
	internal static class SubworldDataStorage
	{

		public static SubworldData Sun = new()
		{
			DisplayName = "The Sun",
			Gravity = 28f,
			Radius = 695700f,
			DayPeriod = 0f,
			ThreatLevel = 10f
 		};

		public static SubworldData Vulcan = new()
		{
			DisplayName = "Vulcan",
			Gravity = 0f,
			Radius = 0f,
			DayPeriod = 0f,
			ThreatLevel = 0f
		};

		public static SubworldData Mercury = new() 
		{ 
			DisplayName = "Mercury",
			Gravity = 0.38f,
			Radius = 2439.7f,
			DayPeriod = 58f,
			ThreatLevel = 0f
		};

		public static SubworldData Venus = new()
		{
			DisplayName = "Venus",
			Gravity = 0.9f,
			Radius = 6051.8f,
			DayPeriod = 116f,
			ThreatLevel = 0f
		};
 
		public static SubworldData Mars = new()
		{
			DisplayName = "Mars",
			Gravity = 0.38f,
			Radius = 3389.5f,
			DayPeriod = 24.62296f,
			ThreatLevel = 3
 		};

		public static SubworldData Jupiter = new()
		{
			DisplayName = "Jupiter",
			Gravity = 2.52f,
			Radius = 69911f,
			DayPeriod = 0.3f,
			ThreatLevel = 0f
		};

		public static SubworldData Saturn = new()
		{
			DisplayName = "Saturn",
			Gravity = 1.065f,
			Radius = 58232f,
			DayPeriod = 0.43f,
			ThreatLevel = 0f      
		};

		public static SubworldData Ouranos = new()
		{
			DisplayName = "Ouranos",
			Gravity = 0.89f,
			Radius = 25362f,
			DayPeriod = 0.718f,
			ThreatLevel = 0f
		};

		public static SubworldData Neptune = new()
		{
			DisplayName = "Neptune",
			Gravity = 1.14f,
			Radius = 24622f,
			DayPeriod = 0.671f,
			ThreatLevel = 0f
		};

		public static SubworldData Pluto = new()
		{
			DisplayName = "Pluto",
			Gravity = 0.064f,
			Radius = 1188.3f,
			DayPeriod = 153.3f,
			ThreatLevel = 0f
		};

		public static SubworldData Eris = new()
		{
			DisplayName = "Eris",
			Gravity = 0.44f,
			Radius = 1163f,
			DayPeriod = 0f,
			ThreatLevel = 0f
		};
	}
}
