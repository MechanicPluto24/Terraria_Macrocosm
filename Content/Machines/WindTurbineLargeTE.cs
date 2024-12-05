using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class WindTurbineLargeTE : GeneratorTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineLarge>();
        public override bool PoweredOn => Math.Abs(Utility.WindSpeedScaled) > 0.1f;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            MaxGeneratedPower = 4f;
            GeneratedPower = PoweredOn ? MaxGeneratedPower * Math.Abs(Utility.WindSpeedScaled) : 0;
        }

        public override void DrawMachinePowerInfo(SpriteBatch spriteBatch, Vector2 basePosition, Color lightColor)
        {
            basePosition.X += 4;
            basePosition.Y -= 52;
            base.DrawMachinePowerInfo(spriteBatch, basePosition, lightColor);
        }
    }
}
