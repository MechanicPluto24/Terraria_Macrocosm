using Macrocosm.Common.Systems.Flags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Macrocosm.Common.Conditions;

public class CustomConditions
{
    public static Condition BloodMoonOrDemonSun => ConditionChain.Any(Condition.BloodMoon, DynamicConditions.Get<WorldData>(nameof(WorldData.DemonSun)));
}
