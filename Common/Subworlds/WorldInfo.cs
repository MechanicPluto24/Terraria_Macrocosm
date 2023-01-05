using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Subworlds
{
    /// <summary> Stores information about Macrocosm Subworlds </summary>
    public struct WorldInfo
    {
        public WorldInfo() { }

        public string DisplayName = "";

        public string FlavorText = "";

        public WorldInfoValue Gravity = new(0f, UnitType.Gravity);
        public WorldInfoValue Radius = new(0f, UnitType.Radius);
        public WorldInfoValue DayPeriod = new(0f, UnitType.DayPeriod);

        /// <summary> 
        /// The threat level of this subworld. Use <see cref = "GetThreatLevelText()"/> 
        /// to <c>get</c> the text associated with this threat level and 
        /// <see cref = "GetThreatLevelCombined"/> for combined formatting.
        /// </summary>
        public int ThreatLevel = 0;

        // TODO: add localization support, which might not require a dictionary
        /// <summary> 
        /// The hazards of this subworld.
        /// Dictionary of string keys used for identifying icons, values being the displayed text
        /// </summary>
        public Dictionary<string, string> Hazards = new();
        public List<string> GetHazardKeys() => Hazards.Keys.ToList();

        // TODO: add localization support
        public string GetThreatLevelText()
            => ThreatLevel < threatLevelToText.Length ? threatLevelToText[ThreatLevel] : threatLevelToText[0];

        public string GetThreatLevelCombined()
            => ThreatLevel.ToString() + " - " + GetThreatLevelText();

        public Color GetThreatColor()
        {
            if (ThreatLevel <= 3)
                return Color.White;
            else if (ThreatLevel <= 6)
                return Color.Yellow;
            else if (ThreatLevel <= 9)
                return Color.Red;
            else
                return Color.Purple;
        }

        private string[] threatLevelToText =
        {
            "NONE",
            "Harmless",
            "Arduous",
            "Challenging",
            "Dangerous",
            "Treacherous",
            "Lethal",
            "Cataclysmic",
            "Nightmare",
            "Apollyon",
            "Unknown"
        };
    }
}

