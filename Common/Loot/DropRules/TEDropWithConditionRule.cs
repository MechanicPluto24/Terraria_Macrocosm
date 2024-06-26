﻿using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropRules
{
    public class TEDropWithConditionRule : TECommonDrop
    {
        public IItemDropRuleCondition Condition;

        public TEDropWithConditionRule(TileEntity tileEntity, int itemId, int chanceDenominator, IItemDropRuleCondition condition, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
            : base(tileEntity, itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
            Condition = condition;
        }

        public override bool CanDrop(DropAttemptInfo info) => base.CanDrop(info) && Condition.CanDrop(info);
    }
}
