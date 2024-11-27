using Macrocosm.Common.Config;
using System;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.UI.Navigation.Info
{
    public class RadiusInfoElement : ValueUnitSpecialInfoElement
    {
        public RadiusInfoElement(string specialValueKey) : base(specialValueKey) { }

        public RadiusInfoElement(float value, string specialValueKey = "") : base(value, specialValueKey) { }

        protected override LocalizedText GetLocalizedValueUnitText(ref float value)
        {
            var type = ClientConfig.Instance.UnitSystem;

            // convert to miles
            if (type == ClientConfig.UnitSystemType.Imperial)
                value *= 0.621f;

            value = MathF.Round(value, 2);

            return Language.GetText("Mods.Macrocosm.UI.Rocket.Navigation.Radius.Unit" + type.ToString());
        }
    }
}
