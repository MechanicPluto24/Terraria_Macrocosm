using Macrocosm.Common.Players;
using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.Info
{
    public class BarometerInfoDisplay : InfoDisplay
    {
        public override bool Active()
        {
            return Main.LocalPlayer.GetModPlayer<InfoDisplayPlayer>().Barometer;
        }

        private float pressure;

        public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
        {
            pressure = MathHelper.Lerp(pressure, MacrocosmSubworld.GetAtmosphericDensity(Main.LocalPlayer.Center, checkRooms: true), 0.1f);
            string text = $"{pressure:F2} atm";
            displayColor = Color.White;
            return text;
        }
    }
}
