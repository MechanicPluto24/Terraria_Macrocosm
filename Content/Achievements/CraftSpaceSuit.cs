using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Items.Armor.Astronaut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements
{
    internal class CraftSpaceSuit : CustomAchievement
    {
        public override float Order => 32f;
        public override AchievementCategory Category => AchievementCategory.Collector;

        protected override void SetupConditions()
        {
            AddItemCraftCondition(ModContent.ItemType<AstronautHelmet>());
            AddItemCraftCondition(ModContent.ItemType<AstronautSuit>());
            AddItemCraftCondition(ModContent.ItemType<AstronautLeggings>());
        }
    }
}
