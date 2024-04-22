using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class SolarPanelSmallTE : MachineTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelSmall>();
        public override bool PoweredOn => Main.dayTime;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            if(PoweredOn)
                GeneratedPower = 0.5f;
            else
                GeneratedPower = 0;
        }
    }
}
