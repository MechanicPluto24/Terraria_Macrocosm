using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class BatteryTE : MachineTE
    {
        public float PowerFlow { get; set; }

        /// <summary> Current stored energy </summary>
        public float StoredEnergy { get; set; }

        /// <summary> Maximum storage energy </summary>
        public abstract float EnergyCapacity { get; }

        public override void UpdatePowerState()
        {
            if (PoweredOn && StoredEnergy <= 0f)
                TurnOff(automatic: true);
            else if (!PoweredOn && StoredEnergy > 0f && !ManuallyTurnedOff)
                TurnOn(automatic: true);
        }

        public override Color DisplayColor => Color.Cyan;
        public override string GetPowerInfo()
        {
            string energy = $"{Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Battery").Format($"{StoredEnergy:F2}", $"{EnergyCapacity:F2}")}";

            string flow = PowerFlow >= 0 ? $"+{PowerFlow:F2}" : $"{PowerFlow:F2}";
            flow = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Simple").Format(flow);

            string percent = $"{(StoredEnergy / EnergyCapacity * 100):F2}%"; 
            
            return energy + " (" + percent + "), " + flow;
        }

        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            string flow = PowerFlow >= 0 ? $"+{PowerFlow:F2}" : $"{PowerFlow:F2}";
            flow = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Simple").Format(flow);

            string percent = $"{(StoredEnergy / EnergyCapacity * 100):F2}%";

            Vector2 positionFlow = new Vector2(basePosition.X + (MachineTile.Width * 16f / 2f) - (FontAssets.MouseText.Value.MeasureString(flow).X / 2f) + 8f, basePosition.Y - 22f) - Main.screenPosition;
            Vector2 positionPercent = new Vector2(basePosition.X + (MachineTile.Width * 16f / 2f) - (FontAssets.MouseText.Value.MeasureString(percent).X / 2f) + 8f, basePosition.Y - 22f) - Main.screenPosition;
            Color color = DisplayColor;

            if(PowerFlow != 0)
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, flow, positionFlow - new Vector2(flow.Length, 24), color, 0f, Vector2.Zero, Vector2.One * 0.4f, spread: 1.5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, percent, positionPercent - new Vector2(percent.Length, 0), color, 0f, Vector2.Zero, Vector2.One * 0.4f, spread: 1.5f);
        }
    }
}
