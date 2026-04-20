using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Power;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Events;

namespace Macrocosm.Content.Machines.Generators.Solar;

public abstract class SolarPanelTEBase : GeneratorTE
{
    protected abstract float BaseGeneratedPower { get; }

    public override bool PoweredOn => GetSolarEfficiency() > 0f;

    public override void MachineUpdate()
    {
        MaxGeneratedPower = BaseGeneratedPower;
        GeneratedPower = MaxGeneratedPower * GetSolarEfficiency();
    }

    private float GetSolarEfficiency()
    {
        if (!Main.dayTime || Main.raining || Sandstorm.Happening)
            return 0f;

        float timeEfficiency = GetDayTimeEfficiency();
        float cloudMultiplier = MathHelper.Lerp(1f, 0.5f, MathHelper.Clamp(Main.cloudAlpha, 0f, 1f));
        float locationMultiplier = MacrocosmSubworld.GetSolarPanelPowerMultiplier(Position.ToVector2() * 16f);

        return timeEfficiency * cloudMultiplier * locationMultiplier;
    }

    private static float GetDayTimeEfficiency()
    {
        const float rampUpEnd = 12600f;     // 8:00 AM
        const float rampDownStart = 45000f; // 5:00 PM
        const float dayEnd = 54000f;        // 7:30 PM

        float time = (float)Main.time;
        if (time < rampUpEnd)
            return MathHelper.Clamp(time / rampUpEnd, 0f, 1f);

        if (time < rampDownStart)
            return 1f;

        return MathHelper.Clamp((dayEnd - time) / (dayEnd - rampDownStart), 0f, 1f);
    }
}
