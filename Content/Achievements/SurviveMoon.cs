using Macrocosm.Common.CrossMod;
using Terraria;
using Terraria.Achievements;

namespace Macrocosm.Content.Achievements;

public class SurviveMoon : TMLAchievement
{
    public override float Order => 40f;
    public override AchievementCategory Category => AchievementCategory.Explorer;
    public override bool ShowProgressBar => false;

    protected override void SetupConditions()
    {
        AddValueEvent(Name, (float)(Main.dayLength + Main.nightLength));
    }
}
