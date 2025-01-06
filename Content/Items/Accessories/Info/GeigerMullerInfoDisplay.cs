using Macrocosm.Common.Players;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories.Info
{
    public class GeigerMullerInfoDisplay : InfoDisplay
    {
        public override bool Active()
        {
            return Main.LocalPlayer.GetModPlayer<InfoDisplayPlayer>().GeigerMuller;
        }

        private int timer;
        private void Sounds()
        {
            var irradiationPlayer = Main.LocalPlayer.GetModPlayer<IrradiationPlayer>();
            int ticksPerSecond = (int)(10 / (irradiationPlayer.IrradiationLevel + 0.1));

            if (irradiationPlayer.IrradiationLevel > 0.01f && timer % ticksPerSecond == 0)
                SoundEngine.PlaySound(SoundID.Item11, Main.LocalPlayer.position);
        }

        private float smoothedIrradiationLevel = 0f;
        public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
        {
            timer++;

            if (!Main.gamePaused)
                Sounds();

            var irradiationPlayer = Main.LocalPlayer.GetModPlayer<IrradiationPlayer>();
            float irradiationLevel = irradiationPlayer.IrradiationLevel;

            if (timer % 5 == 0)
                smoothedIrradiationLevel = irradiationLevel;

            float roentgensPerHour = (float)(0.05 * Math.Pow(10, smoothedIrradiationLevel / 2.0));
            string text = $"{roentgensPerHour:F2} R/h";

            if (irradiationLevel >= 4f)
                displayColor = Color.Red;
            else if (irradiationLevel >= 2.5f)
                displayColor = Color.Orange;
            else if (irradiationLevel >= 1f)
                displayColor = Color.GreenYellow;
            else
                displayColor = Color.DarkGreen;

            return text;
        }
    }
}
