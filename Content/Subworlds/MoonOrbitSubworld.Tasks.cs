using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.WorldGeneration.Structures;
using Macrocosm.Content.WorldGeneration.Structures.Orbit.Moon;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds
{
    public partial class MoonOrbitSubworld
    {
        private StructureMap gen_StructureMap;
        private int gen_spawnExclusionRadius;

        [Task]
        private void PrepareTask(GenerationProgress progress)
        {
            gen_StructureMap = new();
            gen_spawnExclusionRadius = 200;
        }

        [Task]
        private void SpawnTask(GenerationProgress progress)
        {
            Main.spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileY = Main.maxTilesY / 2;
        }

        [Task]
        private void AsteroidTask(GenerationProgress progress)
        {
            //I really do not care if they overlap eachother. BUT they do need to protect the area they spawn in -- Clyder
            for (int x = 50; x < (int)Main.maxTilesX - 50; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    // Don't spawn asteroids too close to the spawn area
                    if (Math.Abs(x - Main.spawnTileX) < gen_spawnExclusionRadius &&
                        Math.Abs(y - Main.spawnTileY) < gen_spawnExclusionRadius)
                        continue;


                    if (WorldGen.genRand.NextBool(60000))
                    {
                        //int wallType = VariantWall.WallType<AstrolithWall>(WallSafetyType.Natural);
                        int wallType = 0;
                        BlobTileRunner(x, y, (ushort)TileType<Astrolith>(), 0..8, 1..4, 4..6, 1f, 4, wallType: (ushort)wallType);

                        //very small chance to create a flesh meteor
                        if (WorldGen.genRand.NextBool(20))
                        {
                            ForEachInCircle(
                                x,
                                y,
                                3,
                                (i1, j1) =>
                                {
                                    if (!WorldGen.InWorld(i1, j1))
                                        return;

                                    float iDistance = Math.Abs(x - i1) / (3 * 0.5f);
                                    float jDistance = Math.Abs(y - j1) / (3 * 0.5f);
                                    if (WorldGen.genRand.NextFloat() < iDistance * 0.2f || WorldGen.genRand.NextFloat() < jDistance * 0.2f)
                                        return;

                                    if (Main.tile[i1, j1].HasTile)
                                        FastPlaceTile(i1, j1, TileID.FleshBlock);
                                }
                            );
                        }

                        gen_StructureMap.AddProtectedStructure(new Rectangle(x - 10, y - 10, x + 10, y + 10), padding: 1);
                    }
                }
            }
        }

        [Task]
        private void OreTask(GenerationProgress progress)
        {
            GenerateOre(TileType<Tiles.Ores.ArtemiteOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileType<Tiles.Ores.SeleniteOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileType<Tiles.Ores.DianiteOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
            GenerateOre(TileType<Tiles.Ores.ChandriumOre>(), 0.005, WorldGen.genRand.Next(1, 4), WorldGen.genRand.Next(1, 4), TileType<Astrolith>());
        }

        [Task(weight: 12.0)]
        private void SmoothTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SmoothTask");
            SmoothWorld(progress);
        }

        [Task]
        private void WallCleanupTask(GenerationProgress progress)
        {
            for (int x = 1; x < Main.maxTilesX; x++)
            {
                for (int y = 1; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];

                    if (!tile.HasTile || tile.BlockType != BlockType.Solid)
                    {
                        //if (tile.WallType == VariantWall.WallType<AstrolithWall>())
                        //    tile.WallType = 0;
                    }

                }
            }
        }

        [Task]
        private void SpaceStationTask(GenerationProgress progress)
        {
            Structure module = Structure.Get<BaseSpaceStationModule>();
            Point16 origin = new(Main.spawnTileX + module.Size.X / 2, Main.spawnTileY);
            module.Place(origin, gen_StructureMap, padding: gen_spawnExclusionRadius);
        }

        [Task]
        private void StructureTask(GenerationProgress progress)
        {
            //The cool stuff
            for (int x = 50; x < Main.maxTilesX - 50; x++)
            {
                for (int y = 50; y < Main.maxTilesY - 50; y++)
                {
                    if (WorldGen.genRand.NextBool(50000))
                    {
                        int random = WorldGen.genRand.Next(8);
                        Structure structure = random switch
                        {
                            0 => Structure.Get<LunarianCameoPod>(),
                            1 => Structure.Get<LunarRemnant1>(),
                            2 => Structure.Get<LuminiteOrbitVein1>(),
                            3 => Structure.Get<LuminiteOrbitVein3>(),
                            4 => Structure.Get<LuminiteOrbitVein2>(),
                            5 => Structure.Get<LuminiteOrbitVein4>(),
                            6 => Structure.Get<LunarSatellite1>(),
                            _ => Structure.Get<ManmadePod1>(),
                        };
                        
                        if (gen_StructureMap.CanPlace(new Rectangle(x - 10, y - 10, structure.Size.X + 10, structure.Size.Y + 10)))
                        {
                            structure.Place(new(x, y), gen_StructureMap);
                        }
                    }
                }

            }
        }
    }
}
