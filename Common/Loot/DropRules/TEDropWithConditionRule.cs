using Macrocosm.Common.Systems.Power;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropRules
{
    public class TEDropWithConditionRule : TECommonDrop
    {
        public IItemDropRuleCondition Condition;

        public TEDropWithConditionRule(MachineTE machineTE, int itemId, int chanceDenominator, IItemDropRuleCondition condition, int minAmt = 1, int maxAmt = 1, int chanceNumerator = 1, float multipleEntityFactor = 0.5f, int? multipleEntityMaxDistance = null)
            : base(machineTE, itemId, chanceDenominator, minAmt, maxAmt, chanceNumerator, multipleEntityFactor, multipleEntityMaxDistance)
        {
            Condition = condition;
        }

        public override bool CanDrop(DropAttemptInfo info) => base.CanDrop(info) && Condition.CanDrop(info);
    }
}
