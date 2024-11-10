using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Furniture;
using Macrocosm.Content.Tiles.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.WorldGeneration.Structures.LunarOutposts
{
    public class MiningOutpost : BaseLunarOutpost
    {
        public override void PreAgeRoom(Point16 origin)
        {
            ushort ore = (ushort)new List<int>
            {
                ModContent.TileType<ArtemiteOre>(),
                ModContent.TileType<SeleniteOre>(),
                ModContent.TileType<DianiteOre>(),
                ModContent.TileType<ChandriumOre>()
            }.GetRandom(WorldGen.genRand);


            for (int i = 0; i < WorldGen.genRand.Next(6, 11); i++)
            {
                WorldGen.OreRunner(WorldGen.genRand.Next(origin.X - 50, origin.X + Size.X + 50), WorldGen.genRand.Next(origin.Y - 50, origin.Y + Size.Y + 50), WorldGen.genRand.Next(6, 11), WorldGen.genRand.Next(3, 6), TileID.LunarOre);
                WorldGen.OreRunner(WorldGen.genRand.Next(origin.X - 50, origin.X + Size.X + 50), WorldGen.genRand.Next(origin.Y - 50, origin.Y + Size.Y + 50), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), ore);
            }

            int lamps = WorldGen.genRand.Next(2, 5);
            int attempts = 0;
            int maxAttempts = lamps * 5; 

            while (lamps > 0 && attempts < maxAttempts)
            {
                attempts++;

                int x = WorldGen.genRand.Next(origin.X, origin.X + Size.X);
                int y = WorldGen.genRand.Next(origin.Y, origin.Y + Size.Y);

                bool placed = Utility.TryPlaceObject<ConstructionLight>(x, y, alternate: WorldGen.genRand.Next(2));

                if (placed)
                    lamps--;
            }
        }

    }
}
