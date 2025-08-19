using Macrocosm.Common.Systems.Power;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters;

public class AutocrafterT2TE : AutocrafterTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<AutocrafterT2>();
    public override int OutputSlots => 2;

    public override void MachineUpdate()
    {
        MaxPower = 0.1f;
        base.MachineUpdate();
    }
}
