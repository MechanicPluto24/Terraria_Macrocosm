using Terraria;

namespace Macrocosm.Common.Conditions;

public static class ConditionNot
{
    // Description is the same
    public static Condition Not(this Condition condition) => new(condition.Description.Key, () => !condition.IsMet());
}
