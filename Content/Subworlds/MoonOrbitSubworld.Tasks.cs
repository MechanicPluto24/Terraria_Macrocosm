using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Subworlds.Orbit.Earth;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Macrocosm.Content.Tiles.Walls;
using Macrocosm.Content.WorldGeneration.Structures;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds
{
    public partial class MoonOrbitSubworld
    {
        public StructureMap OrbitStructureMap { get; private set; } = new();

        [Task]
        private void PlaceSpawn(GenerationProgress progress)
        {

            Structure module = new BaseSpaceStationModule();
            int x, y;
            x = (int)(Main.maxTilesX / 2);
            y = (int)(Main.maxTilesY / 2);
            Point16 origin = new(Main.spawnTileX + (int)(module.Size.X / 2), Main.spawnTileY);
            module.Place(origin, null);
        }
        [Task]
        private void Asteroids(GenerationProgress progress)
        {

            //I really do not care if they overlap eachother. BUT they do need to protect the area they spawn in
            for (int x = 50; x < (int)Main.maxTilesX - 50; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    if (WorldGen.genRand.NextBool(80000) && Math.Abs(Main.spawnTileX - x) > 200)
                    {
                        if(WorldGen.genRand.NextBool(3))
                            Utility.BlobTileRunner(x, y, (ushort)TileType<Protolith>(), 0..3, 1..4, 4..6, 1f, 4, wallType: (ushort)WallType<ProtolithWall>());
                        else if(WorldGen.genRand.NextBool(2))
                            Utility.BlobTileRunner(x, y, (ushort)TileType<Cynthalith>(), 0..3, 1..4, 4..6, 1f, 4, wallType: (ushort)WallType<RegolithWall>());
                        else
                            Utility.BlobTileRunner(x, y, (ushort)TileType<Regolith>(), 0..3, 1..4, 4..6, 1f, 4, wallType: (ushort)WallType<RegolithWall>());

                        //very small chance to create a flesh meteor
                        if (WorldGen.genRand.NextBool(20))
                        {
                            ForEachInCircle(
                                         x,
                                         y,
                                         3,
                                         (i1, j1) =>
                                         {
                                             if (CoordinatesOutOfBounds(i1, j1))
                                             {
                                                 return;
                                             }

                                             float iDistance = Math.Abs(x - i1) / (3 * 0.5f);
                                             float jDistance = Math.Abs(y - j1) / (3 * 0.5f);
                                             if (WorldGen.genRand.NextFloat() < iDistance * 0.2f || WorldGen.genRand.NextFloat() < jDistance * 0.2f)
                                             {
                                                 return;
                                             }

                                             if (Main.tile[i1, j1].HasTile)
                                             {
                                                 FastPlaceTile(i1, j1, TileID.FleshBlock);
                                             }
                                         }
                                     );
                        }

                        OrbitStructureMap.AddProtectedStructure(new Rectangle(x - 10, y - 10, x + 10, y + 10), padding: 1);
                    }
                }
            }

            //Let there be ores
            int protolithType = TileType<Protolith>();
            GenerateOre(TileType<Tiles.Ores.ArtemiteOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileType<Tiles.Ores.SeleniteOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileType<Tiles.Ores.DianiteOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);
            GenerateOre(TileType<Tiles.Ores.ChandriumOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), protolithType);


        }
    }
}
