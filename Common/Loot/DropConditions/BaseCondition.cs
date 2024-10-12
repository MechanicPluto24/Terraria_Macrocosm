using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropConditions
{
    public abstract class BaseCondition : IItemDropRuleCondition
    {
        public abstract bool CanDrop(DropAttemptInfo info);

        public virtual bool CanShowItemDropInUI() => true;

        public virtual string GetConditionDescription() => null;

        public IItemDropRuleCondition Not() => new InvertedBaseCondition(this);

        private class InvertedBaseCondition(BaseCondition baseCondition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => !baseCondition.CanDrop(info);
        }
    }
}
