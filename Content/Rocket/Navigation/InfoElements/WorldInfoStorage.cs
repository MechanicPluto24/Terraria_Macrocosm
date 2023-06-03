using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket.Navigation.InfoElements
{
    public class WorldInfoStorage : ILoadable
    {

        private static Dictionary<string, WorldInfo> worldInfoStorage;

        public void Load(Mod mod)
        {
            worldInfoStorage = new Dictionary<string, WorldInfo>();
            LoadData();
        }

        public void Unload()
        {
            worldInfoStorage = null;
        }

        public static WorldInfo GetValue(string key) => worldInfoStorage[key];


        /// <summary>
        /// Populate the World Info data.
        /// 
        /// Supports: 
        ///     WorldInfoElements: generic info elements, specify numeric or string value (e.g. TidallyLocked), and type (Gravity, Radius, Day Lenght etc.) 
        ///     ThreatLevelInfoElement: the threat level info element, either numeric (1-9) or from ThreatLevel enum
        ///     HazardInfoElement: the hazards specific to this world (e.g. MeteorStorms) 
        /// </summary>
        private static void LoadData()
        {
            worldInfoStorage.Add("InnerSolarSystem", new WorldInfo("InnerSolarSystem"));

            worldInfoStorage.Add("Sun", new WorldInfo("Sun", new()
            {
                new WorldInfoElement(28f, InfoType.Gravity),
                new WorldInfoElement(695700f, InfoType.Radius),

                new ThreatLevelInfoElement(ThreatLevel.Apollyon)
            }));

            worldInfoStorage.Add("Vulcan", new WorldInfo("Vulcan", new()
            {
                new WorldInfoElement("TidallyLocked", InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Mercury", new WorldInfo("Mercury", new()
            {
                new WorldInfoElement(0.38f, InfoType.Gravity),
                new WorldInfoElement(2439.7f, InfoType.Radius),
                new WorldInfoElement(58f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Venus", new WorldInfo("Venus", new()
            {
                new WorldInfoElement(0.9f, InfoType.Gravity),
                new WorldInfoElement(6051.8f, InfoType.Radius),
                new WorldInfoElement(116f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Earth", new WorldInfo("Earth", new()
            {
                new WorldInfoElement(1f, InfoType.Gravity),
                new WorldInfoElement(6371f, InfoType.Radius),
                new WorldInfoElement(1f, InfoType.DayPeriod),

                new ThreatLevelInfoElement(ThreatLevel.Harmless)
            }));

            worldInfoStorage.Add("Moon", new WorldInfo("Moon", new()
            {
                new WorldInfoElement(0.125f, InfoType.Gravity),
                new WorldInfoElement(1737.4f, InfoType.Radius),
                new WorldInfoElement(8f, InfoType.DayPeriod),

                new ThreatLevelInfoElement(ThreatLevel.Arduous),

                new HazardInfoElement("MeteorStorms"),
                new HazardInfoElement("SolarStorms")
            }));

            worldInfoStorage.Add("Mars", new WorldInfo("Mars", new()
            {
                new WorldInfoElement(0.38f, InfoType.Gravity),
                new WorldInfoElement(3389.5f, InfoType.Radius),
                new WorldInfoElement(24.62f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Phobos", new WorldInfo("Phobos", new()
            {
                new WorldInfoElement("Unstable", InfoType.Gravity),
            }));

            worldInfoStorage.Add("Deimos", new WorldInfo("Deimos", new()
            {

            }));

            worldInfoStorage.Add("Ceres", new WorldInfo("Ceres", new()
            {

            }));

            worldInfoStorage.Add("Jupiter", new WorldInfo("Jupiter", new()
            {
                new WorldInfoElement(2.52f, InfoType.Gravity),
                new WorldInfoElement(69911f, InfoType.Radius),
                new WorldInfoElement(0.3f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Io", new WorldInfo("Io", new()
            {

            }));

            worldInfoStorage.Add("Europa", new WorldInfo("Europa", new()
            {

            }));

            worldInfoStorage.Add("Saturn", new WorldInfo("Saturn", new()
            {
                new WorldInfoElement(1.065f, InfoType.Gravity),
                new WorldInfoElement(58232f, InfoType.Radius),
                new WorldInfoElement(0.43f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Titan", new WorldInfo("Titan", new()
            {

            }));

            worldInfoStorage.Add("Ouranos", new WorldInfo("Ouranos", new()
            {
                new WorldInfoElement(0.89f, InfoType.Gravity),
                new WorldInfoElement(25362f, InfoType.Radius),
                new WorldInfoElement(0.718f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Miranda", new WorldInfo("Miranda", new()
            {

            }));

            worldInfoStorage.Add("Neptune", new WorldInfo("Neptune", new()
            {
                new WorldInfoElement(1.14f, InfoType.Gravity),
                new WorldInfoElement(24622f , InfoType.Radius),
                new WorldInfoElement(0.671f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Triton", new WorldInfo("Triton", new()
            {

            }));

            worldInfoStorage.Add("Pluto", new WorldInfo("Pluto", new()
            {
                new WorldInfoElement(0.064f, InfoType.Gravity),
                new WorldInfoElement(1188.3f , InfoType.Radius),
                new WorldInfoElement(153.3f, InfoType.DayPeriod)
            }));

            worldInfoStorage.Add("Charon", new WorldInfo("Charon", new()
            {

            }));

            worldInfoStorage.Add("Eris", new WorldInfo("Eris", new()
            {

            }));

            worldInfoStorage.Add("YanoMoore", new WorldInfo("YanoMoore", new()
            {

            }));
        }
    }
}
