using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets
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

            if (fuelLookup.TryGetValue(key, out float value))
                return value;

            return float.MaxValue;
        }

        private static void Add(string locationKey, string destinationKey, float value)
            => fuelLookup.Add(locationKey + "_" + destinationKey, value);

        private static void PopulateTable()
        {
            Add("Earth", "Earth", 20f);
            Add("Moon", "Moon", 12f);

            Add("Earth", "Moon", 200f);
            Add("Moon", "Earth", 120f);
        }
    }
}
