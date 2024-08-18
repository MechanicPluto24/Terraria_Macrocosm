using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class WindTurbineLargeTE : MachineTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineLarge>();
        public override bool PoweredOn => Math.Abs(Utility.WindSpeedScaled) > 0.1f;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            if (PoweredOn)
                GeneratedPower = 3f * Math.Abs(Utility.WindSpeedScaled);
            else
                GeneratedPower = 0;
        }
    }
}
