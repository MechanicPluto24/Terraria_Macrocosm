using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenActions
{
	// By GroxTheGreat
	public class PlaceModWall : GenAction
    {
        public ushort type;
        public bool neighbors;
        public Func<int, int, Tile, bool> canReplace;

        public PlaceModWall(int type, bool neighbors = true)
        {
            this.type = (ushort)type;
            this.neighbors = neighbors;
        }

        public PlaceModWall ExtraParams(Func<int, int, Tile, bool> canReplace)
        {
            this.canReplace = canReplace;
            return this;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY) return false;
            if (canReplace == null || canReplace != null && canReplace(x, y, _tiles[x, y]))
            {
                _tiles[x, y].WallType = type;
                WorldGen.SquareWallFrame(x, y);
                if (neighbors)
                {
                    WorldGen.SquareWallFrame(x + 1, y);
                    WorldGen.SquareWallFrame(x - 1, y);
                    WorldGen.SquareWallFrame(x, y - 1);
                    WorldGen.SquareWallFrame(x, y + 1);
                }
            }
            return UnitApply(origin, x, y, args);
        }
    }
}