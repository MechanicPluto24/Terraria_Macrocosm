using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenConditions
{
	// By GroxTheGreat
	public class IsSloped : GenCondition
    {
        protected override bool CheckValidity(int x, int y)
        {
            return _tiles[x, y].HasTile && (_tiles[x, y].Slope > 0 || _tiles[x, y].IsHalfBlock);
        }
    }
}