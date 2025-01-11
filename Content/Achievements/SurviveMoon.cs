using Macrocosm.Common.CrossMod;
using Macrocosm.Content.NPCs.Bosses.CraterDemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Achievements;
using Terraria.ModLoader;

namespace Macrocosm.Content.Achievements
{
    public class SurviveMoon : CustomAchievement
    {
        public override float Order => 40f;
        public override AchievementCategory Category => AchievementCategory.Explorer;
        public override bool ShowProgressBar => false;

        protected override void SetupConditions()
        {
            AddValueEvent(Name, (float)(Main.dayLength + Main.nightLength));
        }
    }
}
