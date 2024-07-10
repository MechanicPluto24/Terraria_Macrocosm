using Macrocosm.Common.WorldGeneration;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using Macrocosm.Content.Tiles.Ores;
using Terraria.ID;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines
{
    public class HeavenforgeShrine : Structure
    {
        public override bool PrePlace(Point16 origin)
        {
            return true;
        }

        public override void PostPlace(Point16 origin)
        {
            int min = WorldGen.genRand.Next(15);
            int max = WorldGen.genRand.Next(15, 26);
            for (int vein = min; vein < max; vein++)
            {
                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(1f)),
                    strength: WorldGen.genRand.Next(1, 4),
                    steps: WorldGen.genRand.Next(1, 3),
                    type: (ushort)ModContent.TileType<ArtemiteOre>()
                );

                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(1f)),
                    strength: WorldGen.genRand.Next(1, 4),
                    steps: WorldGen.genRand.Next(1, 3),
                    type: TileID.LunarBlockSolar
                );
            }
        }
    }
}
