using Macrocosm.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropConditions
{
    #pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    public class SceneMetricsConditions
    {
        public abstract class BaseCondition(SceneMetrics sceneMetrics) : IItemDropRuleCondition
        {
            public abstract bool CanDrop(DropAttemptInfo info);
            public virtual bool CanShowItemDropInUI() => true;
            public virtual string GetConditionDescription() => null;
        }

        public class IsPurity(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) =>
                !sceneMetrics.EnoughTilesForCorruption &&
                !sceneMetrics.EnoughTilesForCrimson &&
                !sceneMetrics.EnoughTilesForDesert &&
                !sceneMetrics.EnoughTilesForGlowingMushroom &&
                !sceneMetrics.EnoughTilesForHallow &&
                !sceneMetrics.EnoughTilesForJungle &&
                !sceneMetrics.EnoughTilesForSnow;
        }

        public class IsCorruption(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForCorruption;
        }

        public class IsCrimson(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForCrimson;
        }

        public class IsDesert(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForDesert;
        }

        public class IsHallow(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForHallow;
        }

        public class IsGlowingMushroom(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForGlowingMushroom;
        }

        public class IsJungle(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForJungle;
        }

        public class IsJungleHardmode(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForJungle && Main.hardMode;
        }

        public class IsSnow(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics: sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => sceneMetrics.EnoughTilesForSnow;
        }

        public class NotSnow(SceneMetrics sceneMetrics) : BaseCondition(sceneMetrics: sceneMetrics)
        {
            public override bool CanDrop(DropAttemptInfo info) => !sceneMetrics.EnoughTilesForSnow;
        }
    }
    #pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
}
