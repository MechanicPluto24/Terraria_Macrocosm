﻿using Macrocosm.Common.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Macrocosm.Content.WorldGeneration.Structures.Shrines
{
    public class LuminiteShrine : Structure
    {
        public override bool PrePlace(Point16 origin)
        {
            int max = WorldGen.genRand.Next(15, 26);
            for (int vein = 0; vein < max; vein++)
            {
                WorldGen.OreRunner(
                    i: origin.X + (int)(Size.X * WorldGen.genRand.NextFloat(1f)),
                    j: origin.Y + (int)(Size.Y * WorldGen.genRand.NextFloat(1f)),
                    strength: WorldGen.genRand.Next(2, 5),
                    steps: WorldGen.genRand.Next(4, 8),
                    type: TileID.LunarOre
                );
            }
            return true;
        }

        public override void PostPlace(Point16 origin)
        {
        }
    }
}
