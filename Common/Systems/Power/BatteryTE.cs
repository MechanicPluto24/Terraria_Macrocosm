using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macrocosm.Common.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Localization;
using Terraria.UI.Chat;
using Macrocosm.Common.Utils;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class BatteryTE : MachineTE
    {
        /// <summary> Current stored energy </summary>
        public float StoredEnergy { get; set; }

        /// <summary> Maximum storage energy </summary>
        public abstract float EnergyCapacity { get; }

        public override void UpdatePowerState()
        {
            if (PoweredOn && StoredEnergy <= 0f)
                TurnOff();
            else if (!PoweredOn && StoredEnergy > 0f)
                TurnOn();
        }

        public override Color DisplayColor => Color.Cyan;
        public override string GetPowerInfo() => $"{Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Battery").Format($"{StoredEnergy:F2}", $"{EnergyCapacity:F2}")}";
        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            string active = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Energy").Format($"{StoredEnergy:F0}");
            string total = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Energy").Format($"{EnergyCapacity:F0}");
            string line = new('_', Math.Max(active.Length, total.Length) / 2);

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(total);
            Vector2 position = new Vector2(basePosition.X + (MachineTile.Width * 16f / 2f) - (textSize.X / 2f) + 8f, basePosition.Y - 22f) - Main.screenPosition;
            Color color = Utility.Colorize(DisplayColor, lightColor);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, active, position - new Vector2(active.Length, 24), color, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, line, position - new Vector2(line.Length + 5, 22), color, 0f, Vector2.Zero, Vector2.One);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, total, position - new Vector2(total.Length, 0), color, 0f, Vector2.Zero, Vector2.One);
        }
    }
}
