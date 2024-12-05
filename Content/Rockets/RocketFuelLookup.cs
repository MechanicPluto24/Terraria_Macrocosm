using Macrocosm.Common.Subworlds;
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
            location = MacrocosmSubworld.SanitizeID(location, out _);
            destination = MacrocosmSubworld.SanitizeID(destination, out _);

            string key = location + "_" + destination;

            if (fuelLookup.TryGetValue(key, out float value))
                return value;

            return float.MaxValue;
        }

        private static void Add(string locationKey, string destinationKey, float value)
            => fuelLookup.Add(locationKey + "_" + destinationKey, value);

        private static void PopulateTable()
        {
            // Earth 
            Add("Earth", "Earth", 20f); 
            Add("Earth", "Moon", 200f); 
            Add("Earth", "Sun", 5900f);
            Add("Earth", "Mercury", 3200f);
            Add("Earth", "Venus", 1900f);
            Add("Earth", "Mars", 1300f);
            Add("Earth", "Phobos", 1250f);
            Add("Earth", "Deimos", 1250f);
            Add("Earth", "AsteroidBelt", 1800f);
            Add("Earth", "Ceres", 1900f);
            Add("Earth", "Jupiter", 2800f);
            Add("Earth", "Io", 2900f);
            Add("Earth", "Europa", 2800f);
            Add("Earth", "Saturn", 3700f);
            Add("Earth", "Titan", 3800f);
            Add("Earth", "Ouranos", 4700f);
            Add("Earth", "Miranda", 4800f);
            Add("Earth", "Neptune", 5700f);
            Add("Earth", "Triton", 5800f);
            Add("Earth", "Pluto", 6700f);
            Add("Earth", "Charon", 6800f);
            Add("Earth", "Eris", 7700f);

            // Moon 
            Add("Moon", "Moon", 12f); 
            Add("Moon", "Earth", 120f);   
            Add("Moon", "Sun", 5800f);
            Add("Moon", "Mercury", 3100f);
            Add("Moon", "Venus", 2000f);
            Add("Moon", "Mars", 1500f);
            Add("Moon", "Phobos", 1450f);
            Add("Moon", "Deimos", 1450f);
            Add("Moon", "AsteroidBelt", 1900f);
            Add("Moon", "Ceres", 2000f);
            Add("Moon", "Jupiter", 2800f);
            Add("Moon", "Io", 2900f);
            Add("Moon", "Europa", 2800f);
            Add("Moon", "Saturn", 3700f);
            Add("Moon", "Titan", 3800f);
            Add("Moon", "Ouranos", 4700f);
            Add("Moon", "Miranda", 4800f);
            Add("Moon", "Neptune", 5700f);
            Add("Moon", "Triton", 5800f);
            Add("Moon", "Pluto", 6700f);
            Add("Moon", "Charon", 6800f);
            Add("Moon", "Eris", 7700f);

        }
    }
}
