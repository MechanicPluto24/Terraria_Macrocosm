using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class IndustrialBatteryTE : BatteryTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<IndustrialBattery>();

        public override float EnergyCapacity => 750;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
        }
    }
}
