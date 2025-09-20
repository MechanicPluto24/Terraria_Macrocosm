using Macrocosm.Common.CrossMod;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements;

public class DefeatCraterDemon : TMLAchievement
{
    public override float Order => 40;
    public override AchievementCategory Category => AchievementCategory.Slayer;

    protected override void SetupConditions()
    {
        AddKillNPCCondition(ModContent.NPCType<CraterDemon>());
    }
}
