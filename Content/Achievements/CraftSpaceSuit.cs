using Macrocosm.Common.CrossMod;
using Macrocosm.Content.Items.Armor.Astronaut;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements;

internal class CraftSpaceSuit : TMLAchievement
{
    public override float Order => 32f;
    public override AchievementCategory Category => AchievementCategory.Collector;
    public override bool ShowProgressBar => false;

    protected override void SetupConditions()
    {
        AddItemCraftCondition(ModContent.ItemType<AstronautHelmet>());
        AddItemCraftCondition(ModContent.ItemType<AstronautSuit>());
        AddItemCraftCondition(ModContent.ItemType<AstronautLeggings>());
    }
}
