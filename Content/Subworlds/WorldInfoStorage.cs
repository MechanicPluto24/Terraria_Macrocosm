using System.Collections.Generic;

namespace Macrocosm.Content.Subworlds
{
	internal static class WorldInfoStorage
	{

		public static WorldInfo Sun = new()
		{
			DisplayName = "The Sun",
			Gravity = 28f,
			Radius = 695700f,
			DayPeriod = 0f,
			ThreatLevel = 9
		};

		public static WorldInfo Vulcan = new()
		{
			DisplayName = "Vulcan",
			Gravity = 0f,
			Radius = 0f,
			DayPeriod = "Tidally Locked",
			ThreatLevel = 0
		};

		public static WorldInfo Mercury = new()
		{
			DisplayName = "Mercury",
			Gravity = 0.38f,
			Radius = 2439.7f,
			DayPeriod = 58f,
			ThreatLevel = 0
		};

		public static WorldInfo Venus = new()
		{
			DisplayName = "Venus",
			Gravity = 0.9f,
			Radius = 6051.8f,
			DayPeriod = 116f,
			ThreatLevel = 0
		};

		public static WorldInfo Mars = new()
		{
			DisplayName = "Mars",
			Gravity = 0.38f,
			Radius = 3389.5f,
			DayPeriod = 24.62f,
			ThreatLevel = 3
		};

		public static WorldInfo Jupiter = new()
		{
			DisplayName = "Jupiter",
			Gravity = 2.52f,
			Radius = 69911f,
			DayPeriod = 0.3f,
			ThreatLevel = 0
		};

		public static WorldInfo Saturn = new()
		{
			DisplayName = "Saturn",
			Gravity = 1.065f,
			Radius = 58232f,
			DayPeriod = 0.43f,
			ThreatLevel = 0
		};

		public static WorldInfo Ouranos = new()
		{
			DisplayName = "Ouranos",
			Gravity = 0.89f,
			Radius = 25362f,
			DayPeriod = 0.718f,
			ThreatLevel = 0
		};

		public static WorldInfo Neptune = new()
		{
			DisplayName = "Neptune",
			Gravity = 1.14f,
			Radius = 24622f,
			DayPeriod = 0.671f,
			ThreatLevel = 0
		};

		public static WorldInfo Pluto = new()
		{
			DisplayName = "Pluto",
			Gravity = 0.064f,
			Radius = 1188.3f,
			DayPeriod = 153.3f,
			ThreatLevel = 0
		};

		public static WorldInfo Eris = new()
		{
			DisplayName = "Eris",
			Gravity = 0.44f,
			Radius = 1163f,
			DayPeriod = 0f,
			ThreatLevel = 0
		};

		public static WorldInfo InnerSolarSystem = new()
		{
			DisplayName = "    Inner\nSolar System"
		};
	}
}
