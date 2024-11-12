using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines
{
    public class CryocoreShrine : Structure
    {
        public override bool PrePlace(Point16 origin)
        {
            Utility.SafeTileRunner(origin.X + Size.X / 2, origin.Y + Size.Y / 4, Size.X * 1.3, 1, -1);
            return true;
        }

        public override void PostPlace(Point16 origin)
        {
            int max = WorldGen.genRand.Next(15, 26);
            for (int vein = 0; vein < max; vein++)
            {
                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(2f)),
                    strength: WorldGen.genRand.Next(2, 5),
                    steps: WorldGen.genRand.Next(4, 8),
                    type: (ushort)ModContent.TileType<DianiteOre>()
                );

                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(2f)),
                    strength: WorldGen.genRand.Next(2, 5),
                    steps: WorldGen.genRand.Next(4, 8),
                    type: TileID.LunarBlockNebula
                );
            }
        }
    }
}
