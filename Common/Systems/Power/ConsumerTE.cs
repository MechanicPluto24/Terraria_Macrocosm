using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Macrocosm.Common.Systems.Power
{
    public abstract class ConsumerTE : MachineTE
    {
        public float InputPower { get; set; }

        public float MinPower { get; set; } = 0f;
        public float MaxPower { get; set; }
        public float PowerProgress => MathHelper.Clamp(InputPower / MaxPower, 0f, 1f);

        public override void UpdatePowerState()
        {
            if (PoweredOn && InputPower <= MinPower)
                TurnOff(automatic: true);
            else if (!PoweredOn && InputPower > MinPower && !ManuallyTurnedOff)
                TurnOn(automatic: true);
        }

        public override void OnPowerDisconnected()
        {
            InputPower = 0;
        }

        public override Color DisplayColor => InputPower >= MinPower ? Color.Orange : Color.Orange.WithLuminance(0.5f);
        public override string GetPowerInfo() => $"{Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Consumer").Format($"{InputPower:F2}", $"{MaxPower:F2}")}";

        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            string active = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Simple").Format($"{InputPower:F2}");
            string total = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Simple").Format($"{MaxPower:F2}");
            string line = new('_', Math.Max(active.Length, total.Length) / 2);

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(total);
            Vector2 position = new Vector2(basePosition.X + (MachineTile.Width * 16f / 2f) - (textSize.X / 2f) + 8f, basePosition.Y - 22f) - Main.screenPosition;
            Color color = DisplayColor;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, active, position - new Vector2(active.Length, 24), color, 0f, Vector2.Zero, Vector2.One * 0.4f, spread: 1.5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, line, position - new Vector2(line.Length + 5, 22), color, 0f, Vector2.Zero, Vector2.One * 0.4f, spread: 1.5f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, total, position - new Vector2(total.Length, 0), color, 0f, Vector2.Zero, Vector2.One * 0.4f, spread: 1.5f);
        }
    }
}
