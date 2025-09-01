using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Macrocosm.Common.Loot.DropConditions
{
    /// <summary> Condtion for a specific subworld </summary>
    public class SubworldDropCondition<T> : BaseCondition where T : Subworld
    {
        public override bool CanDrop(DropAttemptInfo info) => SubworldSystem.AnyActive() && ModContent.GetInstance<T>().Name == MacrocosmSubworld.Current.Name;
        public override string GetConditionDescription() => "";
    }
}
