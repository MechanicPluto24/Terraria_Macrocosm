using Macrocosm.Common.Systems.Power;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropRules
{
    public class TEDropWithConditionRule : TECommonDrop
    {
        public IItemDropRuleCondition Condition;

        public TEDropWithConditionRule(MachineTE machineTE, int itemId, int chanceDenominator, IItemDropRuleCondition condition, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1, float multipleEntityFactor = 0.5f, int? multipleEntityMaxDistance = null)
            : base(machineTE, itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator, multipleEntityFactor, multipleEntityMaxDistance)
        {
            Condition = condition;
        }

        public override bool CanDrop(DropAttemptInfo info) => base.CanDrop(info) && Condition.CanDrop(info);
    }
}
