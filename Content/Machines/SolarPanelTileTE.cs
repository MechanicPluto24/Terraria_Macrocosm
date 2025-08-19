using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines;

public class SolarPanelTileTE : GeneratorTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelTile>();

    public override bool PoweredOn => Main.dayTime;
    public override bool CanCluster => true;

    public override void MachineUpdate()
    {
        MaxGeneratedPower = 0.1f * ClusterSize;
        GeneratedPower = PoweredOn ? MaxGeneratedPower : 0;
    }
}
