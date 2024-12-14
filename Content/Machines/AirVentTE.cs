using Macrocosm.Common.Global.Items;
using Macrocosm.Common.Systems.Power;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class AirVentTE : MachineTE, IOxygenSource
    {
        public override MachineTile MachineTile => ModContent.GetInstance<AirVent>();

        public bool IsProvidingOxygen => PoweredOn;

        public override void UpdatePowerState()
        {
            bool oxygen = (wireCircuit != null && wireCircuit.Where((te) => te is OxygenSystemTE oxygenSystem && oxygenSystem.PoweredOn).Any());
            if (!PoweredOn && oxygen)
                TurnOn(automatic: true);
            else if(PoweredOn && !oxygen)
                TurnOff(automatic: true);
        }

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
        }
    }
}
