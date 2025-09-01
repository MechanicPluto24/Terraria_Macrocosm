using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Macrocosm.Common.Loot.DropConditions
{
    /// <summary> Condition for global item drops (e.g. Souls of Night) </summary>
    public class GlobalItemDropCondition : BaseCondition
    {
        public override bool CanDrop(DropAttemptInfo info)
        {
            if (info.npc.boss)
                return false;

            if (NPCID.Sets.CannotDropSouls[info.npc.type])
                return false;

            if (info.npc.value < 1f)
                return false;

            if (info.npc.friendly)
                return false;

            return true;
        }
    }
}
