using Terraria.ModLoader;
using Terraria;
using SubworldLibrary;

namespace Macrocosm {
    public class MacrocosmSystem : ModSystem {
        internal static void INTERNAL_SubworldTileFraming() {
            for (int i = 0; i < Main.maxTilesX; i++) {
                for (int j = Main.maxTilesY - 180; j < Main.maxTilesY; j++) {
                    if (Framing.GetTileSafely(i, j).HasTile)
                        WorldGen.SquareTileFrame(i, j);
                    if (Main.tile[i, j] != null)
                        WorldGen.SquareWallFrame(i, j);
                }
            }
        }

        private bool _anySubworldActive;
        private bool _anySubworldActiveLastTick;
        public override void PostUpdateEverything() {
            _anySubworldActive = SubworldSystem.AnyActive(Macrocosm.Instance);
            if (_anySubworldActive && !_anySubworldActiveLastTick)
                INTERNAL_SubworldTileFraming();

            _anySubworldActiveLastTick = _anySubworldActive;
        }
    }
}