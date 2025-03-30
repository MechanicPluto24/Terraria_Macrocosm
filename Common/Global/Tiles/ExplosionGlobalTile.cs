using SubworldLibrary;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Tiles
{
    public class ExplosionGlobalTile : GlobalTile
    {
        public override bool CanExplode(int i, int j, int type)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                return false;
            else
                return base.CanExplode(i, j, type);
        }
    }
}
