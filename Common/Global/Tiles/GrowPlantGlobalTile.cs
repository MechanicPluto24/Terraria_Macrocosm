using Macrocosm.Content.Tiles.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Tiles
{
    internal class GrowPlantGlobalTile : GlobalTile
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            if(type == TileID.JungleGrass)
            {
                bool aboveGround = j < Main.worldSurface + 10;
                if (aboveGround)
                {
                    Tile tileAbove = Main.tile[i, j - 1];
                    if (WorldGen.genRand.NextBool(700) && (!tileAbove.HasTile || tileAbove.TileType is TileID.JunglePlants or TileID.JunglePlants2 or TileID.JungleThorns))
                        WorldGen.TryGrowingTreeByType(ModContent.TileType<RubberTree>(), i, j);
                }
            }
        }
    }
}
