using System;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Common.DataStructures
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

            private int? count4Way;
            public int Count4Way => count4Way ??= (Top ? 1 : 0)
                                        + (Bottom ? 1 : 0)
                                        + (Right ? 1 : 0)
                                        + (Left ? 1 : 0);
        }

        public record PredicateNeighbourInfo(int I, int J, Func<Tile, bool> Predicate) : CountableNeighbourInfo(I, J)
        {
            protected override bool ShouldCount(Tile tile) => Predicate.Invoke(tile);
        }

        private PredicateNeighbourInfo hasTile;
        public PredicateNeighbourInfo HasTile => hasTile ??= new(I, J, tile => tile.HasTile);

        private PredicateNeighbourInfo solid;
        public PredicateNeighbourInfo Solid => solid ??= new(I, J, tile => tile.HasTile && tile.BlockType == BlockType.Solid);

        private PredicateNeighbourInfo sloped;
        public PredicateNeighbourInfo Sloped => sloped ??= new(I, J, tile => tile.HasTile && tile.BlockType != BlockType.Solid);

        private PredicateNeighbourInfo wall;
        public PredicateNeighbourInfo Wall => wall ??= new(I, J, tile => tile.WallType != WallID.None);

        public PredicateNeighbourInfo TypedSolid(ushort type) => new(I, J, tile => tile.HasTile && tile.TileType == type);
        public PredicateNeighbourInfo TypedSolid(params ushort[] types) => new(I, J, tile => tile.HasTile && types.Contains(tile.TileType));

        public PredicateNeighbourInfo GetPredicateNeighbourInfo(Func<Tile, bool> predicate) => new(I, J, predicate);
    }
}
