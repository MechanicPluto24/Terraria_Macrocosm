using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.GameContent.Events;

namespace Macrocosm.Content.Machines.Generators.Wind;

public abstract class WindTurbineTEBase : GeneratorTE
{
    protected abstract float BaseGeneratedPower { get; }
    protected virtual int WindCheckHeight => MachineTile.Height - 1;

    public override bool PoweredOn => GetWindEfficiency() > 0f && WorldGen.InAPlaceWithWind(Position.X, Position.Y, MachineTile.Width, WindCheckHeight);

    public override void MachineUpdate()
    {
        MaxGeneratedPower = BaseGeneratedPower;
        GeneratedPower = PoweredOn ? MaxGeneratedPower * GetWindEfficiency() : 0f;
    }

    protected float GetWindEfficiency()
    {
        float windSpeed = Math.Clamp(Math.Abs(Utility.WindSpeedScaled) * 100f, 0f, 100f);
        if (windSpeed <= 0f)
            return 0f;

        bool boosted = Main.IsItAHappyWindyDay || Main.IsItStorming || Sandstorm.Happening;
        float efficiencyPercent = boosted && windSpeed >= 20f
            ? 8f * MathF.Sqrt(windSpeed - 20f) + 100f // y = 8 * sqrt(x-20) + 100 
            : 47f * MathF.Pow(windSpeed, 1 / 4f);     // y = 47 * root4(x)
                                                  
        return efficiencyPercent / 100f;
    }
}
