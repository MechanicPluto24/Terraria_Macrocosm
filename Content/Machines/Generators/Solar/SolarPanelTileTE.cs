using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Solar;

public class SolarPanelTileTE : SolarPanelTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelTile>();
    public override bool CanCluster => true;
    protected override float BaseGeneratedPower => 0.1f * ClusterSize;
}
