﻿using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket
{
	public class RocketFuelLookup : ILoadable
	{
		private static Dictionary<string, float> fuelLookup;

		public void Load(Mod mod)
		{
			fuelLookup = new();
			PopulateTable();
		}

		public void Unload()
		{
			fuelLookup = null;
		}

		public static float GetFuelCost(string location, string destination)
		{
			string key = location + "_" + destination;
			
			if(fuelLookup.TryGetValue(key, out float value))
				return value;

			return 0f;
		}


		private static void Add(string locationKey, string destinationKey, float value) 
			=> fuelLookup.Add(locationKey + "_" + destinationKey, value);

		private static void PopulateTable()
		{
			Add("Earth", "Moon", 200f);
			Add("Moon", "Earth", 120f);
		}
	}
}