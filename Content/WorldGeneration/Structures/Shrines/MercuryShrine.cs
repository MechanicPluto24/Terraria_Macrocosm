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
            for(int vein = WorldGen.genRand.Next(6); vein < WorldGen.genRand.Next(13); vein++)
            {
                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(0.5f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(0.2f)),
                    strength: WorldGen.genRand.Next(1, 2),
                    steps: WorldGen.genRand.Next(1, 8),
                    type: (ushort)ModContent.TileType<ArtemiteOre>()
                );

                WorldGen.OreRunner(
                    i: origin.X + Size.X - (int)(Size.X * WorldGen.genRand.NextFloat(0.5f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(0.2f)),
                    strength: WorldGen.genRand.Next(1, 2),
                    steps: WorldGen.genRand.Next(1, 8),
                    type: TileID.LunarBlockSolar
                );
            }
        }
    }
}
