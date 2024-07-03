using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class WindTurbineSmallTE : MachineTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineSmall>();
        public override bool PoweredOn => Math.Abs(Utility.WindSpeedScaled) > 0.1f;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            if(PoweredOn)
                GeneratedPower = 2f * Math.Abs(Utility.WindSpeedScaled);
            else
                GeneratedPower = 0;
        }
    }
}
