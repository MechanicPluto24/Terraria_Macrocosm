using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters
{
    public class AutocrafterT3TE : AutocrafterTEBase
    {
        public override MachineTile MachineTile => ModContent.GetInstance<AutocrafterT3>();

        public override void MachineUpdate()
        {
            base.MachineUpdate();
            RequiredPower = 0.1f;
        }
    }
}
