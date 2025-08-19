using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Machines.Generators.Wind;

public class WindTurbineSmallTE : GeneratorTE
{
    public override MachineTile MachineTile => ModContent.GetInstance<WindTurbineSmall>();
    public override bool PoweredOn => Math.Abs(Utility.WindSpeedScaled) > 0.1f && WorldGen.InAPlaceWithWind(Position.X, Position.Y, MachineTile.Width, MachineTile.Height - 1);

    public override void OnFirstUpdate()
    {
    }

    public override void MachineUpdate()
    {
        MaxGeneratedPower = 1f;
        GeneratedPower = PoweredOn ? MaxGeneratedPower * Math.Abs(Utility.WindSpeedScaled) : 0;
    }
}
