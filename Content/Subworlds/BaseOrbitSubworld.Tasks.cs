using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Walls;
using Macrocosm.Content.WorldGeneration.Structures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds;

public class BaseOrbitSubworld
{

    public static void CommonGen(GenerationProgress progress, StructureMap gen_StructureMap, List<ushort> OreTypes, int gen_spawnExclusionRadius = 200, bool fleshMeteors = false)
    {
        // I really do not care if they overlap eachother. BUT they do need to protect the area they spawn in -- Clyder
        for (int x = 50; x < (int)Main.maxTilesX - 50; x++)
        {
            for (int y = 50; y < Main.maxTilesY - 50; y++)
            {
                // Don't spawn asteroids too close to the spawn area
                if (Math.Abs(x - Main.spawnTileX) < gen_spawnExclusionRadius)
                    continue;


                if (WorldGen.genRand.NextBool(100000))
                {
                    int wallType = VariantWall.WallType<AstrolithWall>(WallSafetyType.Natural);
                    BlobTileRunner(x, y, (ushort)TileType<Astrolith>(), 3..5, 2..4, 6..12, 2f, 5, wallType: (ushort)wallType);
                    if (WorldGen.genRand.NextBool(2))
                    {
                        int radius = WorldGen.genRand.Next(3, 6);
                        ForEachInCircle(
                            x,
                            y,
                            radius + 2,
                            (i1, j1) =>
                            {
                                if (!WorldGen.InWorld(i1, j1))
                                    return;
                                FastPlaceTile(i1, j1, (ushort)TileType<Astrolith>());
                            }
                        );
                        ushort ore = OreTypes[x % OreTypes.Count];
                        ForEachInCircle(
                            x,
                            y,
                            radius,
                            (i1, j1) =>
                            {
                                if (!WorldGen.InWorld(i1, j1))
                                    return;
                                FastPlaceTile(i1, j1, ore);
                            }
                        );
                    }

                    //very small chance to create a flesh meteor
                    if (fleshMeteors)
                    {
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
                    }

                    gen_StructureMap.AddProtectedStructure(new Rectangle(x - 10, y - 10, x + 10, y + 10), padding: 1);
                }
            }
        }

        SmoothWorld(progress);

        int spaceX = (int)(Main.maxTilesX / 2);
        int spaceY = (int)(Main.maxTilesY / 2);
        Structure.Get<BaseSpaceStationModule>().Place(new(spaceX + 5, spaceY), null);
    }
}
