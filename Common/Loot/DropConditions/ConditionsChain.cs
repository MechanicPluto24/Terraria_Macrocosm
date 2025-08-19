using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropConditions;

public class ConditionsChain : IItemDropRuleCondition
{
    private readonly List<IItemDropRuleCondition> conditions;
    private readonly Func<bool[], bool> evaluator;

    protected ConditionsChain(Func<bool[], bool> evaluator, params IItemDropRuleCondition[] conditions)
    {
        this.conditions = conditions.ToList();
        this.evaluator = evaluator;
    }

    public class All(params IItemDropRuleCondition[] conditions) : ConditionsChain(results => results.All(result => result), conditions) { }
    public class Any(params IItemDropRuleCondition[] conditions) : ConditionsChain(results => results.Any(result => result), conditions) { }
    public ConditionsChain Not() => new(results => !evaluator(results), conditions.ToArray());

    public bool CanDrop(DropAttemptInfo info)
    {
        var results = conditions.Select(condition => condition.CanDrop(info)).ToArray();
        return evaluator(results);
    }

    public bool CanShowItemDropInUI() => conditions.All(c => c.CanShowItemDropInUI());

    public string GetConditionDescription()
    {
        var descriptions = conditions.Select(c => c.GetConditionDescription()).Where(description => description is not null);
        return string.Join(", ", descriptions);
    }

    public void Add(IItemDropRuleCondition condition)
    {
        conditions.Add(condition);
    }
}
