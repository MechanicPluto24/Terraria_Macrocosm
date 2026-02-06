using System.Collections.Generic;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements;

public class SurviveMoon : ModAchievement
{
    public CustomFloatCondition Condition { get; private set; }

    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Explorer);
        Condition = AddFloatCondition((float)(Main.dayLength + Main.nightLength));
    }

    public override IEnumerable<Position> GetModdedConstraints()
    {
        yield return new After(ModContent.GetInstance<TravelToMoon>());
    }
}
