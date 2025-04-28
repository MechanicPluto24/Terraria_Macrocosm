using Macrocosm.Common.Config;
using Macrocosm.Common.Enums;
using System;
using Terraria.Localization;

namespace Macrocosm.Common.UI.Rockets.Navigation.Info
{
    public class GravityInfoElement : ValueUnitSpecialInfoElement
    {
        public GravityInfoElement(string specialValueKey) : base(specialValueKey) { }

        public GravityInfoElement(float value, string specialValueKey = "") : base(value, specialValueKey) { }

        protected override LocalizedText GetLocalizedValueUnitText(ref float value)
        {
            bool inGs = ClientConfig.Instance.DisplayGravityInGs;
            UnitSystemType unitType = ClientConfig.Instance.UnitSystem;
            if (!inGs)
            {
                // Approx of Earth's gravitational acceleration, m/s^2
                value *= 9.8f;

                // Convert to feet/s^2
                if (unitType == UnitSystemType.Imperial)
                    value *= 3.28084f;
            }

            value = MathF.Round(value, 3);

            string unit = inGs ? "G" : unitType.ToString();
            return Language.GetText("Mods.Macrocosm.UI.Rocket.Navigation.Gravity.Unit" + unit);
        }
    }
}
