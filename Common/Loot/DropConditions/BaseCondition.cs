using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropConditions
{
    public abstract class BaseCondition : IItemDropRuleCondition
    {
        public abstract bool CanDrop(DropAttemptInfo info);

        public virtual bool Visible { get; init; } = true;
        public bool CanShowItemDropInUI() => Visible;

        public virtual string GetConditionDescription() => null;

        public BaseCondition Not() => new InvertedBaseCondition(this);
        private class InvertedBaseCondition(BaseCondition @base) : BaseCondition { public override bool CanDrop(DropAttemptInfo info) => !@base.CanDrop(info); }
    }
}
