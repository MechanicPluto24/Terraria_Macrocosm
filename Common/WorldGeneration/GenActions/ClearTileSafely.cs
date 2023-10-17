using Terraria.WorldBuilding;
using Terraria;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.WorldGeneration.GenActions
{
	// By GroxTheGreat
	public class ClearTileSafely : GenAction
    {
        private bool _frameNeighbors;

        public ClearTileSafely(bool frameNeighbors = false)
        {
            _frameNeighbors = frameNeighbors;
        }

        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                return false;
            _tiles[x, y].ClearTile();
            if (_frameNeighbors)
            {
                WorldGen.TileFrame(x + 1, y);
                WorldGen.TileFrame(x - 1, y);
                WorldGen.TileFrame(x, y + 1);
                WorldGen.TileFrame(x, y - 1);
            }
            return UnitApply(origin, x, y, args);
        }
    }
}
