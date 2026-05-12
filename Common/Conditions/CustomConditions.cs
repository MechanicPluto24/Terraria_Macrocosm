using Macrocosm.Content.Events;
using Terraria;

namespace Macrocosm.Common.Conditions;

public class CustomConditions
{
    public static Condition BloodMoonOrDemonSun => ConditionChain.Any(Condition.BloodMoon, DynamicConditions.Get<DemonSunEvent>(nameof(DemonSunEvent.Active)));
}
