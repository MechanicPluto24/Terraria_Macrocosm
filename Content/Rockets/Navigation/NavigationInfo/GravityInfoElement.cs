using Macrocosm.Common.Config;
using System;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.Navigation.NavigationInfo
{
    public class GravityInfoElement : ValueUnitSpecialInfoElement
    {
        public GravityInfoElement(string specialValueKey) : base(specialValueKey) { }

        public GravityInfoElement(float value, string specialValueKey = "") : base(value, specialValueKey) { }

        protected override LocalizedText GetLocalizedValueUnitText(ref float value)
        {
            bool inGs = MacrocosmConfig.Instance.DisplayGravityInGs;
            var type = MacrocosmConfig.Instance.UnitSystem;

            string unit = inGs ? "G" : type.ToString();

            if (!inGs)
            {
                // approximate value of Earth's gravitational acceleration
                value *= 9.8f;

                // convert to feet/s^2
                if (type == MacrocosmConfig.UnitSystemType.Imperial)
                    value *= 3.28084f;

                value = MathF.Round(value, 3);
            }

            return Language.GetText("Mods.Macrocosm.UI.Rocket.Gravity.Unit" + unit);
        }
    }
}
