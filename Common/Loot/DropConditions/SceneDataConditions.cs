using Macrocosm.Common.DataStructures;
using Terraria;
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
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForCorruption;
        }

        public class IsCrimson(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForCrimson;
        }

        public class IsHallow(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForHallow;
        }

        public class IsDesert(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForDesert;
        }

        public class IsGlowingMushroom(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForGlowingMushroom;
        }

        public class IsJungle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForJungle;
        }

        public class IsSnow(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForSnow;
        }

        public class IsUnderworld(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.TilePosition.Y > Main.UnderworldLayer;
        }

        public class IsShimmerScene(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForShimmer;
        }

        public class IsMeteor(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForMeteor;
        }

        public class IsWaterCandle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.WaterCandleCount > 0;
        }

        public class IsPeaceCandle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.PeaceCandleCount > 0;
        }

        public class IsShadowCandle(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.ShadowCandleCount > 0;
        }

        public class IsGraveyard(SceneData scene) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => scene.Terraria.EnoughTilesForGraveyard;
        }
    }
}
