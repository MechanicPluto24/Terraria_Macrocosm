using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.Power.Oxygen;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Consumers.Oxygen;

public class OxygenSystemTE : ConsumerTE, IOxygenActiveSource
{
    public override MachineTile MachineTile => ModContent.GetInstance<OxygenSystem>();

    public bool IsProvidingOxygen
    {
        get => PoweredOn;
        set { }
    }

    public int MaxPassiveSources => 5;
    public int MaxRoomSize => 1500;

    public override void OnFirstUpdate()
    {
    }

    public override void MachineUpdate()
    {
        MaxPower = MinPower = 5f;
    }
}
