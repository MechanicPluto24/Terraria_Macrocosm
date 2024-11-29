using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class SolarPanelLargeTE : GeneratorTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelLarge>();
        public override bool PoweredOn => Main.dayTime;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            MaxGeneratedPower = 2f;
            GeneratedPower = PoweredOn ? MaxGeneratedPower : 0;
        }
    }
}
