using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Steam;

public class SteamEngineTE : GeneratorTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<SteamEngine>();

    public override void MachineUpdate()
    {
        MaxGeneratedPower = 3f; // simple constant output
        GeneratedPower = PoweredOn ? MaxGeneratedPower : 0f; // no auto turn on
    }
}

