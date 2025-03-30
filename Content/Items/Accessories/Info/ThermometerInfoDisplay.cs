using Macrocosm.Common.Config;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Players;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.Info
{
    public class ThermometerInfoDisplay : InfoDisplay
    {
        public override bool Active()
        {
            return Main.LocalPlayer.GetModPlayer<InfoDisplayPlayer>().Thermometer;
        }

        private float temperature;

        public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
        {
            temperature = MathHelper.Lerp(temperature, MacrocosmSubworld.GetAmbientTemperature(), 0.1f);
            string text = ClientConfig.Instance.UnitSystem switch
            {
                UnitSystemType.Metric => Language.GetText("Mods.Macrocosm.Machines.Common.TemperatureMetric").Format((int)temperature),
                UnitSystemType.Imperial => Language.GetText("Mods.Macrocosm.Machines.Common.TemperatureImperial").Format((int)Utility.CelsiusToFarhenheit(temperature)),
                _ => "",
            };
            displayColor = Color.White;
            return text;
        }
    }
}
