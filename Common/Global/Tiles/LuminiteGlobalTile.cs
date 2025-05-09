using Macrocosm.Common.Hooks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Tiles
{
    public class LuminiteGlobalTile : GlobalTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSpelunker[TileID.LunarOre] = true;
            Main.tileOreFinderPriority[TileID.LunarOre] = 900;

            PickPowerHook.RegisterPickPowerModification(TileID.LunarOre, minPick: 210, mineResist: 5, onlyInMacrocosm: false);
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (type == TileID.LunarOre)
                return false;
            else
                return base.CanExplode(i, j, type);
        }

        /*
        public override bool? IsTileSpelunkable(int i, int j, int type)
        {
            if (type is TileID.LunarOre && SubworldSystem.AnyActive<Macrocosm>())
                return true;
            else
                return null;
        }
        */
    }
}
