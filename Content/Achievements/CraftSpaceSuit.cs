using System.Collections.Generic;
using Macrocosm.Content.Items.Armor.Astronaut;
using Terraria.Achievements;
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

    public override IEnumerable<Position> GetModdedConstraints()
    {
        yield return new After(ModContent.GetInstance<BuildRocket>());
    }
}
