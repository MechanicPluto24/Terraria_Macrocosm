using Macrocosm.Content.Tiles.Ambient;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Subworlds.Moon.Generation {
    public class AmbientPass : GenPass {
        public AmbientPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) {
            progress.Message = "Placing rocks on the moon...";

            int skipX = 0;
            for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++) {

                progress.Set(tileX / Main.maxTilesX);

                if(skipX > 0) {
                    skipX--;
                    continue;
                }

                if (Main.rand.NextBool(10)) {
                    for (int tileY = 1; tileY < Main.maxTilesY; tileY++) {
                        if (Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY].BlockType == Terraria.ID.BlockType.Solid) {
                            WorldGen.PlaceTile(tileX, tileY - 1, (ushort)ModContent.TileType<RegolithRockSmall>(), true, true, style: Main.rand.Next(10));
                            skipX = Main.rand.Next(10, 50);
                            break;
                        }
                    }
                }
            }
        }
    }
}