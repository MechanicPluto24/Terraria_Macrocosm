

using Terraria.UI;
using Microsoft.Xna.Framework.Input;
using System;
using System.Globalization;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.UI.Rocket.WorldInformation
{
    public enum InfoType
    {
        None,
        Gravity,
        Radius,
        DayPeriod,
		Hazard
    }

	public enum ThreatLevel
	{
		None,
		Harmless,
		Arduous,
		Challenging,
		Dangerous,
		Treacherous,
		Lethal,
		Cataclysmic,
		Nightmare,
		Apollyon,
		Unknown
	}

    public class WorldInfoElement
    {
        private readonly string typeKey = "None";

		public string TypeKey { get => typeKey; }
		public InfoType InfoType { get { int idx = Array.IndexOf(predefTypeToKey, typeKey); return idx == -1 ? InfoType.None : (InfoType)idx; } }
 
		private float value = float.MinValue;
        private string specialValueKey = "default";

        public bool HasValue => value != float.MinValue;
		public bool HasSpecial => specialValueKey != "default";

		public WorldInfoElement(string specialValueKey, string typeKey)
        {
			this.specialValueKey = specialValueKey;
			this.typeKey = typeKey;
        }

        public WorldInfoElement(float value, string typeKey)
        {
			this.value = value;
			this.typeKey = typeKey;
        }

		public WorldInfoElement(float value, string specialValueKey, string typeKey) 
		{
			this.value = value;
			this.specialValueKey = specialValueKey;
			this.typeKey = typeKey;
		}

        public WorldInfoElement(string specialValueKey, InfoType type) : this(specialValueKey, predefTypeToKey[(int)type]) { }
		public WorldInfoElement(float value, InfoType type) : this(value, predefTypeToKey[(int)type]) { }

		public WorldInfoElement(ThreatLevel level) : this((float)level, "Threat" + ((int)level).ToString(), "ThreatLevel") { }


		
		private readonly string localizationPath = "Mods.Macrocosm.WorldInfo.";

        public UIWorldInfoElement ProvideUI() 
        {
            if(!HasValue && !HasSpecial)
                return null;

            string hoverText = Language.GetTextValue(localizationPath + typeKey + ".Name");

            return new UIWorldInfoElement(GetIcon(), GetText(), hoverText, GetUnitKey(), GetTextColor());
		}

		private Texture2D GetIcon()
		{
			string iconPath = "Macrocosm/Content/UI/Rocket/Icons/"; 
			AssetRequestMode mode = AssetRequestMode.ImmediateLoad;

			if (typeKey == "Hazard")
				iconPath += specialValueKey;
			else
				iconPath += typeKey;

			return ModContent.Request<Texture2D>(iconPath, mode).Value;
		}

		private string GetText()
		{
			string formattedValue = string.Format("{0:n}", value).TrimEnd('0').TrimEnd('.');
			string specialValueText = Language.GetTextValue(localizationPath + typeKey + "." + specialValueKey);

			if (HasSpecial)
			{
				if (!HasValue)
					return specialValueText;

				return formattedValue + " - " + specialValueText;
			}

			return formattedValue;
		}

		private Color GetTextColor()
        {
			if(typeKey == "ThreatLevel")
			{
				if (value <= 3)
					return Color.White;
				else if (value <= 6)
					return Color.Yellow;
				else if (value <= 9)
					return Color.Red;
				else
					return Color.Purple;
			}
			else
			{
				return Color.White;
			}
        }

		private string GetUnitKey()
		{
			if (HasSpecial)
				return "";

			string unitKey = (value != 1) ? ".Unit" : ".UnitSingular";
			return Utility.GetLanguageValueOrEmpty(localizationPath + typeKey + unitKey);
		}

		private static string[] predefTypeToKey = new string[]
        {
            "None",
            "Gravity",
            "Radius",
            "DayPeriod",
			"Hazard"
		};
    }
}
