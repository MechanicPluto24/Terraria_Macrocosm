using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Rocket.Navigation.InfoElements
{
    public enum InfoType
    {
        None,
        Gravity,
        Radius,
        DayPeriod
    }

    public class WorldInfoElement : BasicInfoElement
    {
        private InfoType InfoType { get; set; }

        public WorldInfoElement(string specialValueKey, InfoType type) : base(specialValueKey)
        {
            InfoType = type;
        }

        public WorldInfoElement(float value, InfoType type) : base(value)
        {
            InfoType = type;
        }

        private string TypeKey => typeToKey[(int)InfoType];

        private readonly string localizationPath = "Mods.Macrocosm.WorldInfo.";

        protected override Texture2D GetIcon()
        {
            string iconPath = "Macrocosm/Content/Rocket/NavigationUI/Icons/";
            AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

            iconPath += TypeKey;

            return ModContent.Request<Texture2D>(iconPath, mode).Value;
        }

        protected override string GetText()
        {
            string formattedValue = string.Format("{0:n}", value).TrimEnd('0').TrimEnd('.');
            string specialValueText = Language.GetTextValue(localizationPath + TypeKey + "." + specialValueLangKey);

            return HasSpecial ? specialValueText : formattedValue;
        }

        protected override string HoverText => Language.GetTextValue(localizationPath + TypeKey + ".Name");

        protected override string Units => GetUnitKey();
        private string GetUnitKey()
        {
            if (HasSpecial)
                return "";

            string unitKey = value != 1 ? ".Unit" : ".UnitSingular";
            return Utility.GetLanguageValueOrEmpty(localizationPath + TypeKey + unitKey);
        }

        private static string[] typeToKey = new string[]
        {
            "None",
            "Gravity",
            "Radius",
            "DayPeriod",
            "Hazard"
        };
    }
}
