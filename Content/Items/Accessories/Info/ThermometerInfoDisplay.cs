using Macrocosm.Common.Config;
using Macrocosm.Common.Players;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            temperature = MathHelper.Lerp(temperature, MacrocosmSubworld.GetAmbientTemperature(Main.LocalPlayer.Center), 0.1f);

            string text = string.Empty;
            if (ClientConfig.Instance.UnitSystem is ClientConfig.UnitSystemType.Metric)
                text = Language.GetText("Mods.Macrocosm.Machines.Common.TemperatureMetric").Format((int)temperature);
            else if (ClientConfig.Instance.UnitSystem is ClientConfig.UnitSystemType.Imperial)
                text = Language.GetText("Mods.Macrocosm.Machines.Common.TemperatureImperial").Format((int)Utility.CelsiusToFarhenheit(temperature));

            displayColor = Color.White;
            return text;
        }
    }
}
