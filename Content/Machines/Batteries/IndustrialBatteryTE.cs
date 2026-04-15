using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Batteries;

public class IndustrialBatteryTE : BatteryTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<IndustrialBattery>();

    public override float EnergyCapacity => 1000f;

    public override void OnFirstUpdate()
    {
    }

    public override void MachineUpdate()
    {
    }
}
