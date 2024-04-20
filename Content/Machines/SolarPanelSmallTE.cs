using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class SolarPanelSmallTE : MachineTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelSmall>();
        public override bool PoweredUp => Main.dayTime;

        public override void OnFirstUpdate()
        {
        }

        public override void MachineUpdate()
        {
            if(PoweredUp)
                GeneratedPower = 0.5f;
            else
                GeneratedPower = 0;
        }
    }
}
