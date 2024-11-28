
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Macrocosm.Content.Tiles.Ores;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Furniture;

namespace Macrocosm.Content.WorldGeneration.Structures.LunarOutposts
{
    public class MiningOutpost2 : BaseLunarOutpost
    {
        public override void PreAgeRoom(Point16 origin)
        {
            Utility.SafeTileRunner(origin.X + Size.X - 10, origin.Y + Size.Y / 2, WorldGen.genRand.Next(20, 26), WorldGen.genRand.Next(50, 71), -1);

            ushort ore = (ushort)new List<int>
            {
                ModContent.TileType<ArtemiteOre>(),
                ModContent.TileType<SeleniteOre>(),
                ModContent.TileType<DianiteOre>(),
                ModContent.TileType<ChandriumOre>()
            }.GetRandom(WorldGen.genRand);

            for (int i = 0; i < WorldGen.genRand.Next(3, 8); i++)
            {
                WorldGen.OreRunner(WorldGen.genRand.Next(origin.X + Size.X - 10, origin.X + Size.X + 50), WorldGen.genRand.Next(origin.Y - 10, origin.Y + Size.Y + 10), WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(3, 6), TileID.LunarOre);
                WorldGen.OreRunner(WorldGen.genRand.Next(origin.X + Size.X - 10, origin.X + Size.X + 50), WorldGen.genRand.Next(origin.Y - 10, origin.Y + Size.Y + 10), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), ore);
            }

            int lamps = WorldGen.genRand.Next(2, 5);
            int attempts = 0;
            int maxAttempts = 1000;

            while (lamps > 0 && attempts < maxAttempts)
            {
                attempts++;

                int x = WorldGen.genRand.Next(origin.X - 50, origin.X + Size.X + 50);
                int y = WorldGen.genRand.Next(origin.Y, origin.Y + Size.Y + 20);

                bool placed = Utility.TryPlaceObject<ConstructionLight>(x, y, alternate: WorldGen.genRand.Next(2));

                if (placed)
                    lamps--;
            }
        }
    }
}
