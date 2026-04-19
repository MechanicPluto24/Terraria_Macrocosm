using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Solar;

public class SolarPanelExtraLargeTE : SolarPanelTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<SolarPanelExtraLarge>();
    protected override float BaseGeneratedPower => 65f;
}
