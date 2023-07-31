using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Macrocosm.Common.Utils;

namespace Macrocosm.Common.Bases
{
    public record TileNeighbourInfo(int I, int J)
    {
        public static bool CoordinatesOutOfBounds(int i, int j) => i >= Main.maxTilesX || j >= Main.maxTilesY || i < 0 || j < 0;

        private bool? top;
        public bool Top => top ??= (CoordinatesOutOfBounds(I, J - 1) || Main.tile[I, J - 1].HasTile);

        private bool? topRight;
        public bool TopRight => topRight ??= (CoordinatesOutOfBounds(I + 1, J - 1) || Main.tile[I + 1, J - 1].HasTile);

        private bool? topleft;
        public bool TopLeft => topleft ??= (CoordinatesOutOfBounds(I - 1, J - 1) || Main.tile[I - 1, J - 1].HasTile);

        private bool? bottom;
        public bool Bottom => bottom ??= (CoordinatesOutOfBounds(I, J + 1) || Main.tile[I, J + 1].HasTile);

        private bool? bottomRight;
        public bool BottomRight => bottomRight ??= (CoordinatesOutOfBounds(I + 1, J + 1) || Main.tile[I + 1, J + 1].HasTile);

        private bool? bottomLeft;
        public bool BottomLeft => bottomLeft ??= (CoordinatesOutOfBounds(I - 1, J + 1) || Main.tile[I - 1, J + 1].HasTile);

        private bool? right;
        public bool Right => right ??= (CoordinatesOutOfBounds(I + 1, J) || Main.tile[I + 1, J].HasTile);

        private bool? left;
        public bool Left => left ??= (CoordinatesOutOfBounds(I - 1, J) || Main.tile[I - 1, J].HasTile);

        private int? solidCount;
        public int SolidCount
        {
            get
            {
                if (solidCount is null)
                {
                    solidCount = 0;
                    for (int x = I - 1; x < I + 2; x++)
                    {
                        for (int y = J - 1; y < J + 2; y++)
                        {
                            if ((x == I && y == J))
                            {
                                continue;
                            }

                            if (CoordinatesOutOfBounds(x, y) || (Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType]))
                            {
                                solidCount++;
                            }
                        }
                    }
                }

                return solidCount.Value;
            }
        }

        private int? wallCount;
        public int WallCount
        {
            get
            {
                if (wallCount is null)
                {
                    wallCount = 0;
                    for (int x = I - 1; x < I + 2; x++)
                    {
                        for (int y = J - 1; y < J + 2; y++)
                        {
                            if ((x == I && y == J))
                            {
                                continue;
                            }

                            if (CoordinatesOutOfBounds(x, y) || Main.tile[x, y].WallType != WallID.None)
                            {
                                wallCount++;
                            }
                        }
                    }
                }


                return wallCount.Value;
            }
        }
    }
}
