using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Macrocosm.Common.Loot
{
    /// <summary> 
    /// Simple loot table that isn't bound to anything (neither NPCs nor Grab Bags). 
    /// Includes its own resolver.
    /// </summary>
    public class SimpleLootTable : ILoot, IEnumerable<IItemDropRule>
    {
        public readonly List<IItemDropRule> Entries;

        public SimpleLootTable()
        {
            Entries = new();
        }

        public IItemDropRule Add(IItemDropRule entry)
        {
            Entries.Add(entry);
            return entry;
        }

        public List<IItemDropRule> Get(bool includeGlobalDrops = true) => Entries;

        public IItemDropRule Remove(IItemDropRule entry)
        {
            Entries.Remove(entry);
            return entry;
        }

        public void RemoveWhere(Predicate<IItemDropRule> predicate, bool includeGlobalDrops = true)
        {
            foreach (var entry in Entries)
            {
                if (predicate(entry))
                    Remove(entry);
            }
        }

        public static readonly DropAttemptInfo CommonDropAttemptInfo = new()
        {
            player = Main.LocalPlayer,
            IsExpertMode = Main.expertMode,
            IsMasterMode = Main.masterMode,
            IsInSimulation = false,
            rng = Main.rand,
            npc = null,
            item = 0
        };

        public void Drop(Player player)
        {
            DropAttemptInfo info = CommonDropAttemptInfo;
            info.player = player;

            foreach (IItemDropRule rule in Entries)
            {
                ResolveRule(rule, info);
            }
        }

        private ItemDropAttemptResult ResolveRule(IItemDropRule rule, DropAttemptInfo info)
        {
            if (!rule.CanDrop(info))
            {
                ItemDropAttemptResult fail = default;
                fail.State = ItemDropAttemptResultState.DoesntFillConditions;
                ResolveRuleChains(rule, info, fail);
                return fail;
            }

            ItemDropAttemptResult result = (rule is not INestedItemDropRule nestedItemDropRule) ? rule.TryDroppingItem(info) : nestedItemDropRule.TryDroppingItem(info, ResolveRule);
            ResolveRuleChains(rule, info, result);
            return result;
        }

        private void ResolveRuleChains(IItemDropRule rule, DropAttemptInfo info, ItemDropAttemptResult parentResult)
        {
            ResolveRuleChains(ref info, ref parentResult, rule.ChainedRules);
        }

        private void ResolveRuleChains(ref DropAttemptInfo info, ref ItemDropAttemptResult parentResult, List<IItemDropRuleChainAttempt> ruleChains)
        {
            if (ruleChains == null)
                return;

            for (int i = 0; i < ruleChains.Count; i++)
            {
                IItemDropRuleChainAttempt itemDropRuleChainAttempt = ruleChains[i];
                if (itemDropRuleChainAttempt.CanChainIntoRule(parentResult))
                    ResolveRule(itemDropRuleChainAttempt.RuleToChain, info);
            }
        }

        public IEnumerator<IItemDropRule> GetEnumerator()
        {
            return ((IEnumerable<IItemDropRule>)Entries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Entries).GetEnumerator();
        }
    }
}
