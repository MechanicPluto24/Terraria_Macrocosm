using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.Power.Oxygen;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines;

public class AirVentTE : OxygenPassiveSourceTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<AirVent>();
    public override int MaxRoomSize => 750;
}
