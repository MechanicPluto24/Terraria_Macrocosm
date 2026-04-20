using Macrocosm.Content.Items.Armor.Astronaut;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements;

internal class CraftSpaceSuit : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Collector);
        AddManyItemCraftCondition([
            ModContent.ItemType<AstronautHelmet>(),
            ModContent.ItemType<AstronautSuit>(),
            ModContent.ItemType<AstronautLeggings>()
        ]);
    }

    public override Position GetDefaultPosition() => new After("TO_INFINITY_AND_BEYOND");
}
