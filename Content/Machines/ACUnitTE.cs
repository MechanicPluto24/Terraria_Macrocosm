using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.Power.Oxygen;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines
{
    public class ACUnitTE : OxygenPassiveSourceTE
    {
        public override MachineTile MachineTile => ModContent.GetInstance<ACUnit>();
        public override int MaxRoomSize => 1500;
    }
}
