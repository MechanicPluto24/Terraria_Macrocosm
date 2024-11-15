using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Macrocosm.Common.Loot.DropConditions
{
    public class TilePositionConditions
    {
        public class IsUnderworldHeight(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => tilePosition.Y > Main.UnderworldLayer;
        }

        public class IsRockLayerHeight(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) =>
                tilePosition.Y <= Main.UnderworldLayer && tilePosition.Y > Main.rockLayer;
        }

        public class IsDirtLayerHeight(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) =>
                tilePosition.Y <= Main.rockLayer && tilePosition.Y > Main.worldSurface;
        }

        public class IsOverworldHeight(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) =>
                tilePosition.Y <= Main.worldSurface && tilePosition.Y > Main.worldSurface * 0.35;
        }

        public class IsSkyHeight(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) =>
                tilePosition.Y <= Main.worldSurface * 0.35;
        }

        public class IsBeach(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => WorldGen.oceanDepths(tilePosition.X, tilePosition.Y);
        }

        public class IsRain(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) =>
                Main.raining && tilePosition.Y <= Main.worldSurface;
        }

        public class IsShimmerHeight(Point tilePosition) : BaseCondition
        {
            public override bool CanDrop(DropAttemptInfo info) => tilePosition.Y > Main.worldSurface + 50 && tilePosition.Y < (float)(Main.maxTilesY - 330 - 100);
        }
    }
}
