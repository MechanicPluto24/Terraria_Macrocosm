using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.DropRules
{
    internal class SubworldDropCondition<T> : IItemDropRuleCondition where T : Subworld
    {
        private readonly bool canShowInBestiary;

        public SubworldDropCondition(bool canShowInBestiary)
        {
            this.canShowInBestiary = canShowInBestiary;
        }

        public bool CanDrop(DropAttemptInfo info) => ModContent.GetInstance<T>().Name == MacrocosmSubworld.Current.Name;

        public bool CanShowItemDropInUI() => canShowInBestiary;

        public string GetConditionDescription() => "";
    }
}
