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

namespace Macrocosm.Common.Systems.Power
{
    public abstract class GeneratorTE : MachineTE
    {
        public float GeneratedPower { get; set; }

        public override void UpdatePowerState()
        {
            if (PoweredOn && GeneratedPower <= 0f)
                PowerOff();
            else if (!PoweredOn && GeneratedPower > 0f)
                PowerOn();
        }

        public override void MachineUpdate()
        {
            base.MachineUpdate();
        }

        public override Color DisplayColor => Color.LimeGreen;

        public override string GetPowerInfo() => $"{Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Generator").Format($"{GeneratedPower:F2}")}";
        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch)
        {
            Vector2 position = (Position.ToWorldCoordinates() + new Vector2(MachineTile.Width * 16f / 4f, 0)) - Main.screenPosition;
            string total = Language.GetText($"Mods.Macrocosm.Machines.Common.PowerInfo.Simple").Format($"{GeneratedPower:F2}");
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, total, position - new Vector2(total.Length, 26), DisplayColor, 0f, Vector2.Zero, Vector2.One);
        }
    }
}
