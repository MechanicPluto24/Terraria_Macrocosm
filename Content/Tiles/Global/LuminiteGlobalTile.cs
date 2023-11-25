using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Global
{
    public class LuminiteGlobalTile : GlobalTile
    {
        public override bool? IsTileSpelunkable(int i, int j, int type)
        {
            if (type is TileID.LunarOre /* && SubworldSystem.AnyActive<Macrocosm>()*/)
                return true;
            else
                return null;
        }
    }
}
