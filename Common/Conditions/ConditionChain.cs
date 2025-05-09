using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Macrocosm.Common.Conditions
{
    public static class ConditionChain
    {
        public static Condition All(params Condition[] conditions)
        {
            var conditionList = conditions.ToList();

            string description = string.Join("\n", conditionList.Select(c => c.Description.Value));
            bool predicate() => conditionList.All(c => c.IsMet());
            return new Condition(description, predicate);
        }

        public static Condition Any(params Condition[] conditions)
        {
            var conditionList = conditions.ToList();
            string description = string.Join("\n", conditionList.Select(c => c.Description.Value));
            bool predicate() => conditionList.Any(c => c.IsMet());
            return new Condition(description, predicate);
        }
    }
}