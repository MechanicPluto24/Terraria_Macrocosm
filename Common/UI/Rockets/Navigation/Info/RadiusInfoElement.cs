using Macrocosm.Common.Config;
using Macrocosm.Common.Enums;
using System;
using Terraria.Localization;

namespace Macrocosm.Common.UI.Rockets.Navigation.Info
{
    public class RadiusInfoElement : ValueUnitSpecialInfoElement
    {
        public RadiusInfoElement(string specialValueKey) : base(specialValueKey) { }

        public RadiusInfoElement(float value, string specialValueKey = "") : base(value, specialValueKey) { }

        protected override LocalizedText GetLocalizedValueUnitText(ref float value)
        {
            UnitSystemType unitType = ClientConfig.Instance.UnitSystem;

            // convert to miles
            if (unitType == UnitSystemType.Imperial)
                value *= 0.621f;

            value = MathF.Round(value, 2);
            return Language.GetText("Mods.Macrocosm.UI.Rocket.Navigation.Radius.Unit" + unitType.ToString());
        }
    }
}
