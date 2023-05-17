using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.UI.Rocket
{
    public class WorldInfoDatabase : ILoadable
    {

        private static Dictionary<string, WorldInfo> database;

        public void Load(Mod mod)
        {
            database = new Dictionary<string, WorldInfo>();
            PopulateDatabase();
        }

        public void Unload()
        {
            database = null;
        }

        public static WorldInfo GetValue(string key) => database[key];

        private static void PopulateDatabase()
        {
            database.Add("InnerSolarSystem", new WorldInfo("InnerSolarSystem"));

            database.Add("Sun", new WorldInfo("Sun", new()
            {
                new WorldInfoElement(28f, InfoType.Gravity),
                new WorldInfoElement(695700f, InfoType.Radius),

                new ThreatLevelInfoElement(ThreatLevel.Apollyon)
            }));

            database.Add("Vulcan", new WorldInfo("Vulcan", new()
            {
                new WorldInfoElement("TidallyLocked", InfoType.DayPeriod)
            }));

            database.Add("Mercury", new WorldInfo("Mercury", new()
            {
                new WorldInfoElement(0.38f, InfoType.Gravity),
                new WorldInfoElement(2439.7f, InfoType.Radius),
                new WorldInfoElement(58f, InfoType.DayPeriod)
            }));

            database.Add("Venus", new WorldInfo("Venus", new()
            {
                new WorldInfoElement(0.9f, InfoType.Gravity),
                new WorldInfoElement(6051.8f, InfoType.Radius),
                new WorldInfoElement(116f, InfoType.DayPeriod)
            }));

            database.Add("Earth", new WorldInfo("Earth", new()
            {
                new WorldInfoElement(1f, InfoType.Gravity),
                new WorldInfoElement(6371f, InfoType.Radius),
                new WorldInfoElement(1f, InfoType.DayPeriod),

                new ThreatLevelInfoElement(ThreatLevel.Harmless)
            }));

            database.Add("Moon", new WorldInfo("Moon", new()
            {
                new WorldInfoElement(0.125f, InfoType.Gravity),
                new WorldInfoElement(1737.4f, InfoType.Radius),
                new WorldInfoElement(8f, InfoType.DayPeriod),

                new ThreatLevelInfoElement(ThreatLevel.Arduous),

                new HazardInfoElement("MeteorStorms"),
                new HazardInfoElement("SolarStorms")
            }));

            database.Add("Mars", new WorldInfo("Mars", new()
            {
                new WorldInfoElement(0.38f, InfoType.Gravity),
                new WorldInfoElement(3389.5f, InfoType.Radius),
                new WorldInfoElement(24.62f, InfoType.DayPeriod)
            }));

            database.Add("Phobos", new WorldInfo("Phobos", new()
            {
                new WorldInfoElement("Unstable", InfoType.Gravity),
            }));

            database.Add("Deimos", new WorldInfo("Deimos", new()
            {

            }));

            database.Add("Ceres", new WorldInfo("Ceres", new()
            {

            }));

            database.Add("Jupiter", new WorldInfo("Jupiter", new()
            {
                new WorldInfoElement(2.52f, InfoType.Gravity),
                new WorldInfoElement(69911f, InfoType.Radius),
                new WorldInfoElement(0.3f, InfoType.DayPeriod)
            }));

            database.Add("Io", new WorldInfo("Io", new()
            {

            }));

            database.Add("Europa", new WorldInfo("Europa", new()
            {

            }));

            database.Add("Saturn", new WorldInfo("Saturn", new()
            {
                new WorldInfoElement(1.065f, InfoType.Gravity),
                new WorldInfoElement(58232f, InfoType.Radius),
                new WorldInfoElement(0.43f, InfoType.DayPeriod)
            }));

            database.Add("Titan", new WorldInfo("Titan", new()
            {

            }));

            database.Add("Ouranos", new WorldInfo("Ouranos", new()
            {
                new WorldInfoElement(0.89f, InfoType.Gravity),
                new WorldInfoElement(25362f, InfoType.Radius),
                new WorldInfoElement(0.718f, InfoType.DayPeriod)
            }));

            database.Add("Miranda", new WorldInfo("Miranda", new()
            {

            }));

            database.Add("Neptune", new WorldInfo("Neptune", new()
            {
                new WorldInfoElement(1.14f, InfoType.Gravity),
                new WorldInfoElement(24622f , InfoType.Radius),
                new WorldInfoElement(0.671f, InfoType.DayPeriod)
            }));

            database.Add("Triton", new WorldInfo("Triton", new()
            {

            }));

            database.Add("Pluto", new WorldInfo("Pluto", new()
            {
                new WorldInfoElement(0.064f, InfoType.Gravity),
                new WorldInfoElement(1188.3f , InfoType.Radius),
                new WorldInfoElement(153.3f, InfoType.DayPeriod)
            }));

            database.Add("Charon", new WorldInfo("Charon", new()
            {

            }));

            database.Add("Eris", new WorldInfo("Eris", new()
            {

            }));

            database.Add("YanoMoore", new WorldInfo("YanoMoore", new()
            {

            }));
        }
    }
}
