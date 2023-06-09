using System.Collections.Generic;
using System.Linq;
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

        public static void Add(string key, params BasicInfoElement[] infoElements)
            => worldInfoStorage.Add(key, new WorldInfo(key, infoElements.ToList()));

        public static WorldInfo GetValue(string key) => worldInfoStorage[key];
        public static bool TryGetValue(string key, out WorldInfo info) 
            => worldInfoStorage.TryGetValue(key, out info);

        /// <summary>
        /// Populate the World Info data.
        /// 
        /// Supports: 
        ///     WorldInfoElements: generic info elements, specify numeric or string value (e.g. TidallyLocked), and type (Gravity, Radius, Day Lenght etc.).
        ///     ThreatLevelInfoElement: the threat level info element, either numeric (1-9) or from ThreatLevel enum.
        ///     HazardInfoElement: the hazards specific to this world (e.g. MeteorStorms).
        ///     
        ///     Icons are loaded based on the name, from Navigation/Icons.
        ///     Text is loaded from the localization files.
        /// </summary>
        private static void LoadData()
        {
            //Add("InnerSolarSystem"
            //    
            //);

            Add("Sun",
                new WorldInfoElement(28f, InfoType.Gravity),
                new WorldInfoElement(695700f, InfoType.Radius),

                new ThreatLevelInfoElement(ThreatLevel.Apollyon)
            );

            Add("Vulcan",
                new WorldInfoElement("TidallyLocked", InfoType.DayPeriod)
            );

            Add("Mercury",
                new WorldInfoElement(0.38f, InfoType.Gravity),
                new WorldInfoElement(2439.7f, InfoType.Radius),
                new WorldInfoElement(58f, InfoType.DayPeriod)
            );

            Add("Venus",
                new WorldInfoElement(0.9f, InfoType.Gravity),
                new WorldInfoElement(6051.8f, InfoType.Radius),
                new WorldInfoElement(116f, InfoType.DayPeriod)
            );

            Add("Earth",  
                new WorldInfoElement(1f, InfoType.Gravity),
                new WorldInfoElement(6371f, InfoType.Radius),
                new WorldInfoElement(1f, InfoType.DayPeriod),

                new ThreatLevelInfoElement(ThreatLevel.Harmless)
            );

            Add("Moon",  
                new WorldInfoElement(0.125f, InfoType.Gravity),
                new WorldInfoElement(1737.4f, InfoType.Radius),
                new WorldInfoElement(8f, InfoType.DayPeriod),

                new ThreatLevelInfoElement(ThreatLevel.Arduous),

                new HazardInfoElement("MeteorStorms"),
                new HazardInfoElement("SolarStorms")
            );

            Add("Mars",  
                new WorldInfoElement(0.38f, InfoType.Gravity),
                new WorldInfoElement(3389.5f, InfoType.Radius),
                new WorldInfoElement(24.62f, InfoType.DayPeriod)
            );

            Add("Phobos", 
                 new WorldInfoElement("Unstable", InfoType.Gravity)
            );

            Add("Deimos" 
 
            );

            Add("Ceres"
 
            );

            Add("Jupiter",  
                new WorldInfoElement(2.52f, InfoType.Gravity),
                new WorldInfoElement(69911f, InfoType.Radius),
                new WorldInfoElement(0.3f, InfoType.DayPeriod)
            );

            Add("Io"
 
            );

            Add("Europa"

            );

            Add("Saturn",
                new WorldInfoElement(1.065f, InfoType.Gravity),
                new WorldInfoElement(58232f, InfoType.Radius),
                new WorldInfoElement(0.43f, InfoType.DayPeriod)
            );

            Add("Titan"

            );

            Add("Ouranos",
                new WorldInfoElement(0.89f, InfoType.Gravity),
                new WorldInfoElement(25362f, InfoType.Radius),
                new WorldInfoElement(0.718f, InfoType.DayPeriod)
            );

            Add("Miranda"

            );

            Add("Neptune",
                new WorldInfoElement(1.14f, InfoType.Gravity),
                new WorldInfoElement(24622f , InfoType.Radius),
                new WorldInfoElement(0.671f, InfoType.DayPeriod)
            );

            Add("Triton"

            );

            Add("Pluto",
                new WorldInfoElement(0.064f, InfoType.Gravity),
                new WorldInfoElement(1188.3f , InfoType.Radius),
                new WorldInfoElement(153.3f, InfoType.DayPeriod)
            );

            Add("Charon"

            );

            Add("Eris"

            );

            Add("YanoMoore"

            );
        }
    }
}
