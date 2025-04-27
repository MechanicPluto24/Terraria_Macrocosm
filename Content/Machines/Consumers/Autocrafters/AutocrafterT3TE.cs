using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Autocrafters
{
    public class AutocrafterT3TE : AutocrafterTEBase
    {
        public override MachineTile MachineTile => ModContent.GetInstance<AutocrafterT3>();
        public override int OutputSlots => 4;
        public override void MachineUpdate()
        {
            base.MachineUpdate();
            MaxPower = 0.1f;
        }
    }
}
