using Macrocosm.Common.CrossMod;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements
{
    public class DefeatCraterDemon : CustomAchievement
    {
        public override float Order => 38;
        public override AchievementCategory Category => AchievementCategory.Slayer;

        protected override void SetupConditions()
        {
            AddKillNPCCondition(ModContent.NPCType<CraterDemon>());
        }
    }
}

