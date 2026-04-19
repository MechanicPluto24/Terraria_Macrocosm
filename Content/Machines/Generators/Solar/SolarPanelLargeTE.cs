using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Solar;

public class SolarPanelLargeTE : SolarPanelTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelLarge>();
    protected override float BaseGeneratedPower => 25f;
}
