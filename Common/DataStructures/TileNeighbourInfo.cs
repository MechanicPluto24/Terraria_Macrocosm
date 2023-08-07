using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Macrocosm.Common.Utils;
using static Macrocosm.Common.Bases.TileNeighbourInfo;

namespace Macrocosm.Common.Bases
{
    public record TileNeighbourInfo(int I, int J)
    {
        private static bool CoordinatesOutOfBounds(int i, int j) => i >= Main.maxTilesX || j >= Main.maxTilesY || i < 0 || j < 0;
        public abstract record CountableNeighbourInfo(int I, int J)
        {
            protected abstract bool ShouldCount(Tile tile);

            private bool? top;
            public bool Top => top ??= (CoordinatesOutOfBounds(I, J - 1) || ShouldCount(Main.tile[I, J - 1]));

            private bool? topRight;
            public bool TopRight => topRight ??= (CoordinatesOutOfBounds(I + 1, J - 1) || ShouldCount(Main.tile[I + 1, J - 1]));

            private bool? topleft;
            public bool TopLeft => topleft ??= (CoordinatesOutOfBounds(I - 1, J - 1) || ShouldCount(Main.tile[I - 1, J - 1]));

            private bool? bottom;
            public bool Bottom => bottom ??= (CoordinatesOutOfBounds(I, J + 1) || ShouldCount(Main.tile[I, J + 1]));

            private bool? bottomRight;
            public bool BottomRight => bottomRight ??= (CoordinatesOutOfBounds(I + 1, J + 1) || ShouldCount(Main.tile[I + 1, J + 1]));

            private bool? bottomLeft;
            public bool BottomLeft => bottomLeft ??= (CoordinatesOutOfBounds(I - 1, J + 1) || ShouldCount(Main.tile[I - 1, J + 1]));

            private bool? right;
            public bool Right => right ??= (CoordinatesOutOfBounds(I + 1, J) || ShouldCount(Main.tile[I + 1, J]));

            private bool? left;
            public bool Left => left ??= (CoordinatesOutOfBounds(I - 1, J) || ShouldCount(Main.tile[I - 1, J]));

            private int? count;
            public int Count => count ??= (Top ? 1 : 0)
                            + (TopRight ? 1 : 0)
                            + (TopLeft ? 1 : 0)
                            + (Bottom ? 1 : 0)
                            + (BottomRight ? 1 : 0)
                            + (BottomLeft ? 1 : 0)
                            + (Right ? 1 : 0)
                            + (Left ? 1 : 0);
        }

        public record SolidInfo(int I, int J) : CountableNeighbourInfo(I, J)
        {
            protected override bool ShouldCount(Tile tile) => tile.HasTile;
        }

        public record WallInfo(int I, int J) : CountableNeighbourInfo(I, J)
        {
            protected override bool ShouldCount(Tile tile) => tile.WallType != WallID.None;
        }

        private SolidInfo solid;
        public SolidInfo Solid => solid ??= new SolidInfo(I, J);

        private WallInfo wall;
        public WallInfo Wall => wall ??= new WallInfo(I, J);
    }
}
