using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Macrocosm.Common.Loot.DropConditions
{
    internal class SubworldDropCondition<T>(bool canShowInBestiary) : IItemDropRuleCondition where T : Subworld
    {
        public bool CanDrop(DropAttemptInfo info) => SubworldSystem.AnyActive() && ModContent.GetInstance<T>().Name == MacrocosmSubworld.Current.Name;

        public bool CanShowItemDropInUI() => canShowInBestiary;

        public string GetConditionDescription() => "";
    }
}
