using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines
{
    public class MercuryShrine : Structure
    {
        public override bool PrePlace(Point16 origin)
        {
            return true;
        }

        public override void PostPlace(Point16 origin)
        {
            int max = WorldGen.genRand.Next(30, 41);
            for (int vein = 0; vein < max; vein++)
            {
                WorldGen.OreRunner(
                    i: origin.X - (int)(Size.X * WorldGen.genRand.NextFloat(0.5f)) + (int)(Size.X * WorldGen.genRand.NextFloat(1.5f)),
                    j: origin.Y - (int)(Size.Y * WorldGen.genRand.NextFloat(0.3f)) + (int)(Size.Y * WorldGen.genRand.NextFloat(1.3f)),
                    strength: WorldGen.genRand.Next(2, 5),
                    steps: WorldGen.genRand.Next(6, 8),
                    type: (ushort)ModContent.TileType<ArtemiteOre>()
                );

                WorldGen.OreRunner(
                    i: origin.X - (int)(Size.X * WorldGen.genRand.NextFloat(0.5f)) + (int)(Size.X * WorldGen.genRand.NextFloat(1.5f)),
                    j: origin.Y - (int)(Size.Y * WorldGen.genRand.NextFloat(0.3f)) + (int)(Size.Y * WorldGen.genRand.NextFloat(1.3f)),
                    strength: WorldGen.genRand.Next(2, 5),
                    steps: WorldGen.genRand.Next(6, 8),
                    type: TileID.LunarBlockSolar
                );
            }
        }
    }
}
