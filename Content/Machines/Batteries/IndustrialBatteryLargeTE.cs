using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Batteries;

public class IndustrialBatteryLargeTE : BatteryTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<IndustrialBatteryLarge>();

    public override float EnergyCapacity => 2500f;  

    public override void OnFirstUpdate()
    {
    }

    public override void MachineUpdate()
    {
    }
}
