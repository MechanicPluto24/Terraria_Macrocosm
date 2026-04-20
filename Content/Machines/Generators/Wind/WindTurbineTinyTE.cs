using Macrocosm.Common.Systems.Power;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Wind;

public class WindTurbineTinyTE : WindTurbineTEBase
{
    public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineTiny>();
    protected override float BaseGeneratedPower => 15f;
}
