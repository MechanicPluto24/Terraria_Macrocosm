using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements;

public class BuildRocket : ModAchievement
{
    public CustomFlagCondition Condition { get; private set; }

    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Explorer);
        Condition = AddCondition();
    }

    public override Position GetDefaultPosition() => new After("TO_INFINITY_AND_BEYOND");
}
