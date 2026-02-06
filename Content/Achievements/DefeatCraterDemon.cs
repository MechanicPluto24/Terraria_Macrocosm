using System.Collections.Generic;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements;

public class DefeatCraterDemon : ModAchievement
{
    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Slayer);
        AddNPCKilledCondition(ModContent.NPCType<CraterDemon>());
    }

    public override IEnumerable<Position> GetModdedConstraints()
    {
        yield return new After(ModContent.GetInstance<SurviveMoon>());
    }
}
