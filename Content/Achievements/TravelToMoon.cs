using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements;

public class TravelToMoon : ModAchievement
{
    public CustomFlagCondition Condition { get; private set; }

    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Explorer);
        Condition = AddCondition();
    }

    public override IEnumerable<Position> GetModdedConstraints()
    {
        yield return new After(ModContent.GetInstance<BuildRocket>());
    }
}
