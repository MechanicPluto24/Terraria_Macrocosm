using System;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.Navigation.NavigationInfo
{
    public class DayPeriodInfoElement : ValueUnitSpecialInfoElement
    {
        public DayPeriodInfoElement(string specialValueKey) : base(specialValueKey) { }

        public DayPeriodInfoElement(float value, string specialValueKey = "") : base(value, specialValueKey) { }

        protected override LocalizedText GetLocalizedValueUnitText(ref float value)
        {
            string units = value < 1f ? "Hours" : "Days";

            if (value < 1f)
                value *= 24f;

            value = MathF.Round(value, 2);

            return Language.GetText("Mods.Macrocosm.UI.Rocket.DayPeriod.Unit" + units);
        }

    }
}
