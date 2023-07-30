using Macrocosm.Common.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
    internal class TestSmoothPass : GenPass
    {
        public TestSmoothPass(string name, double loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int x = 0; x < 5; x++)
            {
                for (int i = 20; i < Main.maxTilesX - 20; i++)
                {
                    for (int j = 20; j < Main.maxTilesY - 20; j++)
                    {
                        TileNeighbourInfo neighbourInfo = new(i, j);
                        if (neighbourInfo.SolidCount > 4)
                        {
                            Main.tile[i, j].Get<TileWallWireStateData>().HasTile = true;
                            if (neighbourInfo.SolidCount < 7)
                            {
                                WorldGen.SlopeTile(i, j);
                            }
                        }
                        else if (neighbourInfo.SolidCount < 4)
                        {
                            Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                        }
                    }
                }
            }
        }
    }
}
