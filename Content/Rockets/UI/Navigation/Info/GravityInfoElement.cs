using Macrocosm.Common.Config;
using System;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.UI.Navigation.Info
{
    public class GravityInfoElement : ValueUnitSpecialInfoElement
    {
        public GravityInfoElement(string specialValueKey) : base(specialValueKey) { }

        public GravityInfoElement(float value, string specialValueKey = "") : base(value, specialValueKey) { }

        protected override LocalizedText GetLocalizedValueUnitText(ref float value)
        {
            bool inGs = ClientConfig.Instance.DisplayGravityInGs;
            var type = ClientConfig.Instance.UnitSystem;

            string unit = inGs ? "G" : type.ToString();

            if (!inGs)
            {
                // approximate value of Earth's gravitational acceleration
                value *= 9.8f;

                // convert to feet/s^2
                if (type == ClientConfig.UnitSystemType.Imperial)
                    value *= 3.28084f;

                value = MathF.Round(value, 3);
            }

            return Language.GetText("Mods.Macrocosm.UI.Rocket.Navigation.Gravity.Unit" + unit);
        }
    }
}
