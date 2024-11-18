using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.Localization;
using Terraria.UI.Chat;
using Macrocosm.Common.Utils;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class GeneratorTE : MachineTE
    {
        public float GeneratedPower { get; set; }

        public override void UpdatePowerState()
        {
            if (PoweredOn && GeneratedPower <= 0f)
                TurnOff();
            else if (!PoweredOn && GeneratedPower > 0f)
                TurnOn();
        }

        public override Color DisplayColor => Color.LimeGreen;

        public override string GetPowerInfo() => $"{Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Generator").Format($"{GeneratedPower:F2}")}";
        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            string total = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Simple").Format($"{GeneratedPower:F2}");

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(total);
            Vector2 position = new Vector2(basePosition.X + (MachineTile.Width * 16f / 2f) - (textSize.X / 2f) + 8f, basePosition.Y - 22f) - Main.screenPosition;
            Color color = Utility.Colorize(DisplayColor, lightColor);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, total, position - new Vector2(total.Length, 0), color, 0f, Vector2.Zero, Vector2.One);
        }
    }
}
