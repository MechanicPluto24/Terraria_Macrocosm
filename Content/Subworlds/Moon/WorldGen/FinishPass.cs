using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Subworlds.Moon.Generation {
    public class FinishPass : GenPass {
        public FinishPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) {

            progress.Message = "Final touches...";

            SetSpawnPoint();
            Main.dayTime = true;

        }

        public void SetSpawnPoint() {
            Main.spawnTileX = Main.maxTilesX / 2;
            for (int tileY = 0; tileY < Main.maxTilesY; tileY++) {
                if (Main.tile[1000, tileY].HasTile) {
                    Main.spawnTileY = tileY - 1;
                    break;
                }
            }
        }
    }
}