using Macrocosm.Common.Systems.Power;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class WindTurbineSmallTE : MachineTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineSmall>();
        public override bool PoweredOn => Math.Abs(Main.windSpeedCurrent) > 0.1f;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            if(PoweredOn)
                GeneratedPower = 2f * Math.Abs(Main.windSpeedCurrent);
            else
                GeneratedPower = 0;
        }
    }
}
