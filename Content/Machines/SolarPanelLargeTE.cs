using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class SolarPanelLargeTE : MachineTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelLarge>();
        public override MachineType MachineType => MachineType.Generator;

        public override bool PoweredOn => Main.dayTime;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            if (PoweredOn)
                Power = 2f;
            else
                Power = 0;
        }
    }
}
