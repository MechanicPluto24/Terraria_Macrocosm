using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Wind;

public class WindTurbineSmallTE : WindTurbineTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineSmall>();
    protected override float BaseGeneratedPower => 60f;
}
