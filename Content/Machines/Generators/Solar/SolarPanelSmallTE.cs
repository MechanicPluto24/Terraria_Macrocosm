using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Solar;

public class SolarPanelSmallTE : SolarPanelTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelSmall>();
    protected override float BaseGeneratedPower => 10f;
}
