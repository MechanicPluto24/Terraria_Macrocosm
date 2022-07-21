using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.ModLoader;

namespace Macrocosm.Content.Subworlds.Moon.Generation
{
    public class BackgroundPass : GenPass
    {
        public BackgroundPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Backgrounding the Moon...";
            for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
            {
                int wall = 2;
                float progressPercent = tileX / Main.maxTilesX;
                progress.Set(progressPercent);
                bool surroundedTile = false;
                for (int tileY = 2; tileY < Main.maxTilesY - 1; tileY++)
                {
                    if (Main.tile[tileX, tileY].HasTile)
                        wall = ModContent.WallType<Walls.RegolithWall>();

                    if (surroundedTile)
                        Main.tile[tileX, tileY].WallType = (ushort)wall;

                    if
                    (
                        Main.tile[tileX, tileY].HasTile // Current tile is active
                        && Main.tile[tileX - 1, tileY].HasTile // Left tile is active
                        && Main.tile[tileX + 1, tileY].HasTile // Right tile is active
                        && Main.tile[tileX, tileY + 1].HasTile // Bottom tile is active
                        && Main.tile[tileX - 1, tileY + 1].HasTile // Bottom-left tile is active
                        && Main.tile[tileX + 1, tileY + 1].HasTile // Bottom-right tile is active
                                                                   // The following will help to make the walls slightly lower than the terrain
                        && Main.tile[tileX, tileY - 2].HasTile // Top tile is active
                    )
                    {
                        surroundedTile = true; // Set the rest of the walls down the column
                    }
                }
            }
        }
    }
}