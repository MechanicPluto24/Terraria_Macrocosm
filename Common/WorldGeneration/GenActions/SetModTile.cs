using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenActions
{
	// By GroxTheGreat
	public class SetModTile : GenAction
    {
        public ushort type;
        public short frameX = -1;
        public short frameY = -1;
        public bool doFraming;
        public bool doNeighborFraming;
        public Func<int, int, Tile, bool> canReplace;

        public SetModTile(ushort type, bool setSelfFrames = false, bool setNeighborFrames = true)
        {
            this.type = type;
            doFraming = setSelfFrames;
            doNeighborFraming = setNeighborFrames;
        }

        public SetModTile ExtraParams(Func<int, int, Tile, bool> canReplace, int frameX = -1, int frameY = -1)
        {
            this.canReplace = canReplace;
            this.frameX = (short)frameX;
            this.frameY = (short)frameY;
            return this;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
                return false;
            if (canReplace == null || canReplace != null && canReplace(x, y, _tiles[x, y]))
            {
                _tiles[x, y].ResetToType(type);
                if (frameX > -1)
                    _tiles[x, y].TileFrameX = frameX;
                if (frameY > -1)
                    _tiles[x, y].TileFrameY = frameY;
                if (doFraming)
                {
                    WorldUtils.TileFrame(x, y, doNeighborFraming);
                }
            }
            return UnitApply(origin, x, y, args);
        }
    }
}