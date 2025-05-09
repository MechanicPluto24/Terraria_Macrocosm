using Macrocosm.Common.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropConditions
{
    public class SceneDataConditions
    {
        public class IsPurity(SceneData scene) : ConditionsChain.All
        (
            new IsCorruption(scene).Not(),
            new IsCrimson(scene).Not(),
            new IsHallow(scene).Not(),
            new IsDesert(scene).Not(),
            new IsSnow(scene).Not(),
            new IsJungle(scene).Not(),
            new IsGlowingMushroom(scene).Not(),
            new IsUnderworld(scene).Not()
        )
        { }

        public class IsEvil(SceneData scene) : ConditionsChain.Any(new IsCorruption(scene), new IsCrimson(scene)) { }

        public class IsSpreadableBiome(SceneData scene) : ConditionsChain.Any(new IsEvil(scene), new IsHallow(scene)) { }

        public class IsCorruption(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneCorrupt;
        }

        public class IsCrimson(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneCrimson;
        }

        public class IsHallow(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneHallow;
        }

        public class IsDesert(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneDesert;
        }

        public class IsGlowingMushroom(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneGlowshroom;
        }

        public class IsJungle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneJungle;
        }

        public class IsSnow(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneSnow;
        }

        public class IsUnderworld(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneUnderworldHeight;
        }

        public class IsShimmerScene(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneShimmer;
        }

        public class IsMeteor(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneMeteor;
        }

        public class IsWaterCandle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneWaterCandle;
        }

        public class IsPeaceCandle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZonePeaceCandle;
        }

        public class IsShadowCandle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneShadowCandle;
        }

        public class IsGraveyard(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.ZoneGraveyard;
        }
    }
}
