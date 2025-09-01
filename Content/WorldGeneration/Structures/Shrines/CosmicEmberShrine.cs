using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Tiles.Furniture.Luminite;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines
{
    public class CosmicEmberShrine : Structure
    {
        public override bool PrePlace(Point16 origin)
        {
            Utility.SafeTileRunner(origin.X + Size.X / 2, origin.Y + Size.Y / 4, Size.X * 2.5f, 1, -1);
            return true;
        }

        public override void PostPlace(Point16 origin)
        {
            for (int i = origin.X; i < origin.X + Size.X; i++)
            {
                for (int j = origin.Y; j < origin.Y + Size.Y; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.TileType == ModContent.TileType<LuminiteChest>())
                        Utility.SetTileStyle(i, j, 17);
                }
            }
            int max = WorldGen.genRand.Next(15, 26);
            for (int vein = 0; vein < max; vein++)
            {
                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(1f)),
                    strength: WorldGen.genRand.Next(2, 5),
                    steps: WorldGen.genRand.Next(4, 8),
                    type: (ushort)ModContent.TileType<ChandriumOre>()
                );

                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(1f)),
                    strength: WorldGen.genRand.Next(2, 5),
                    steps: WorldGen.genRand.Next(4, 8),
                    type: TileID.LunarBlockStardust
                );
            }
        }
    }
}
