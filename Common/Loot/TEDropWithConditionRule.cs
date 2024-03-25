using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.ObjectData;

namespace Macrocosm.Common.Loot
{
    public class TEDropWithConditionRule : TECommonDrop 
    {
        public IItemDropRuleCondition Condition;

        public TEDropWithConditionRule(TileEntity tileEntity, int itemId, int chanceDenominator, IItemDropRuleCondition condition, int amountDroppedMinimum = 1, int amountDroppedMaximum = 1, int chanceNumerator = 1)
            : base(tileEntity, itemId, chanceDenominator, amountDroppedMinimum, amountDroppedMaximum, chanceNumerator)
        {
            Condition = condition;
        }

        public override bool CanDrop(DropAttemptInfo info) => Condition.CanDrop(info);
    }
}
