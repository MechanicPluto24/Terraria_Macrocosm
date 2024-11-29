using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class WindTurbineSmallTE : GeneratorTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineSmall>();
        public override bool PoweredOn => Math.Abs(Utility.WindSpeedScaled) > 0.1f;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            MaxGeneratedPower = 1f;
            GeneratedPower = PoweredOn ? MaxGeneratedPower * Math.Abs(Utility.WindSpeedScaled) : 0;
        }
    }
}
