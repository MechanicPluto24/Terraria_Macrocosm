using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.GenActions
{
	// By GroxTheGreat
	public class IsInWorld : GenAction
    {
        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
                return Fail();
            return UnitApply(origin, x, y, args);
        }
    }
}