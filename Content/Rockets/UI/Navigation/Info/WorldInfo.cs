using Macrocosm.Common.Systems;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.UI.Navigation.Info
{
    public class WorldInfo : ILoadable
    {
        private static Dictionary<string, List<InfoElement>> worldInfoStorage;

        public void Load(Mod mod)
        {
            worldInfoStorage = new Dictionary<string, List<InfoElement>>();
            LoadData();
        }

        public void Unload()
        {
            worldInfoStorage.Clear();
            worldInfoStorage = null;
        }

        public static void Add(string subworldId, params InfoElement[] infoElements)
            => worldInfoStorage.Add(subworldId, infoElements.ToList());

        public static List<InfoElement> GetInfoElements(string subworldId) => worldInfoStorage[subworldId];

        public static bool TryGetValue(string key, out List<InfoElement> info)
            => worldInfoStorage.TryGetValue(key, out info);

        public static LocalizedText GetFlavorText(string subworldId)
        {
            if (subworldId == "Phobos" && WorldDataSystem.Instance.DeimosReturn)
                return Utility.GetLocalizedTextOrEmpty("Mods.Macrocosm.Subworlds." + subworldId + ".FlavorText_DeimosReturn");

            return Utility.GetLocalizedTextOrEmpty("Mods.Macrocosm.Subworlds." + subworldId + ".FlavorText");
        }

        /// <summary>
        /// Populates the World Info data.
        /// <br> <br>
        /// Supports: 
        /// <list type="bullet">
        ///     <item> <see cref="GravityInfoElement"/>, <see cref="RadiusInfoElement"/>, <see cref="DayPeriodInfoElement"/>: general info elements, specify numeric and/or string value (e.g. TidallyLocked) </item>
        ///     <item> <see cref="ThreatLevelInfoElement"/>: the threat level info element, pick from ThreatLevel enum. </item>
        ///     <item> <see cref="HazardInfoElement"/>: the hazards specific to this world (e.g. MeteorStorms). </item>
        /// </list>
        ///          
        ///     <br> Icons are loaded based on the name, from Rockets/Textures/Icons. </br>
        ///     <br> Text is loaded from the localization files. </br>
        /// </br> </br>
        /// </summary>
        private static void LoadData()
        {
            Add("Sun",
                new GravityInfoElement(28f),
                new RadiusInfoElement(695700f),
                new ThreatLevelInfoElement(ThreatLevel.Apollyon, Color.Red)
            );

            Add("Vulcan",
                new DayPeriodInfoElement("TidallyLocked"),
                new ThreatLevelInfoElement(ThreatLevel.Cataclysmic, Color.Orange)
            );

            Add("Mercury",
                new GravityInfoElement(0.38f),
                new RadiusInfoElement(2439.7f),
                new DayPeriodInfoElement(58f),
                new ThreatLevelInfoElement(ThreatLevel.Treacherous, Color.Yellow)
            );

            Add("Venus",
                new GravityInfoElement(0.9f),
                new RadiusInfoElement(6051.8f),
                new DayPeriodInfoElement(116f),
                new ThreatLevelInfoElement(ThreatLevel.Cataclysmic, Color.Orange)
            );

            Add("Earth",
                new GravityInfoElement(1f),
                new RadiusInfoElement(6371f),
                new DayPeriodInfoElement(1f),
                new ThreatLevelInfoElement(ThreatLevel.Harmless, Color.White)
            );

            Add("Moon",
                new GravityInfoElement(0.125f),
                new RadiusInfoElement(1737.4f),
                new DayPeriodInfoElement(8f, "TidallyLocked"),
                new ThreatLevelInfoElement(ThreatLevel.Arduous, Color.White),
                new HazardInfoElement("MeteorStorms"),
                new HazardInfoElement("SolarStorms")
            );

            Add("Mars",
                new GravityInfoElement(0.38f),
                new RadiusInfoElement(3389.5f),
                new DayPeriodInfoElement(24.62f),
                new ThreatLevelInfoElement(ThreatLevel.Challenging, Color.Yellow)
            );

            Add("Phobos",
                new GravityInfoElement("Unstable"),
                new ThreatLevelInfoElement(ThreatLevel.Challenging, Color.Yellow)
            );

            Add("Deimos",
                new ThreatLevelInfoElement(ThreatLevel.Challenging, Color.Yellow)
            );

            Add("AsteroidBelt",
                new ThreatLevelInfoElement(ThreatLevel.Dangerous, Color.Yellow)
            );

            Add("Ceres",
                new ThreatLevelInfoElement(ThreatLevel.Dangerous, Color.Yellow)
            );

            Add("Jupiter",
                new GravityInfoElement(2.52f),
                new RadiusInfoElement(69911f),
                new DayPeriodInfoElement(0.3f),
                new ThreatLevelInfoElement(ThreatLevel.Nightmare, Color.Orange)
            );

            Add("Io",
                new ThreatLevelInfoElement(ThreatLevel.Treacherous, Color.Yellow)
            );

            Add("Europa",
                new ThreatLevelInfoElement(ThreatLevel.Treacherous, Color.Yellow)
            );

            Add("Saturn",
                new GravityInfoElement(1.065f),
                new RadiusInfoElement(58232f),
                new DayPeriodInfoElement(0.43f),
                new ThreatLevelInfoElement(ThreatLevel.Nightmare, Color.Orange)
            );

            Add("Titan",
                new ThreatLevelInfoElement(ThreatLevel.Treacherous, Color.Yellow)
            );

            Add("Ouranos",
                new GravityInfoElement(0.89f),
                new RadiusInfoElement(25362f),
                new DayPeriodInfoElement(0.718f),
                new ThreatLevelInfoElement(ThreatLevel.Nightmare, Color.Orange)
            );

            Add("Miranda",
                new ThreatLevelInfoElement(ThreatLevel.Lethal, Color.Orange)
            );

            Add("Neptune",
                new GravityInfoElement(1.14f),
                new RadiusInfoElement(24622f),
                new DayPeriodInfoElement(0.671f),
                new ThreatLevelInfoElement(ThreatLevel.Nightmare, Color.Orange)
            );

            Add("Triton",
                new ThreatLevelInfoElement(ThreatLevel.Lethal, Color.Orange)
            );

            Add("Pluto",
                new GravityInfoElement(0.064f),
                new RadiusInfoElement(1188.3f),
                new DayPeriodInfoElement(153.3f),
                new ThreatLevelInfoElement(ThreatLevel.Lethal, Color.Orange)
            );

            Add("Charon",
                new ThreatLevelInfoElement(ThreatLevel.Lethal, Color.Orange)
            );

            Add("Eris",
                new ThreatLevelInfoElement(ThreatLevel.Cataclysmic, Color.Orange)
            );

            Add("Nibiru",
                new ThreatLevelInfoElement(ThreatLevel.Unknown, Color.Purple)
            );
        }
    }
}
