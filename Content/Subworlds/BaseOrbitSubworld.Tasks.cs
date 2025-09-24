using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.WorldGeneration.Structures;
using Macrocosm.Content.WorldGeneration.Structures.Orbit.Moon;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Macrocosm.Content.Tiles.Furniture.Luminite;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Accessories;
using Macrocosm.Content.Items.Armor.Vanity.Employee;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Consumables.BossSummons;
using Macrocosm.Content.Items.Consumables.Potions;
using Macrocosm.Content.Items.Consumables.Throwable;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Ores;
using Macrocosm.Content.Items.Refined;
using Macrocosm.Content.Items.Tools.Hammers;
using Macrocosm.Content.Items.Torches;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Enums;

namespace Macrocosm.Content.Subworlds;

public class BaseOrbitSubworld
{
 
    public static void CommonGen(GenerationProgress progress,StructureMap gen_StructureMap,List<ushort> OreTypes,int gen_spawnExclusionRadius = 200, bool FleshMeteors=false)
    {
        //I really do not care if they overlap eachother. BUT they do need to protect the area they spawn in -- Clyder
        for (int x = 50; x < (int)Main.maxTilesX - 50; x++)
        {
            for (int y = 50; y < Main.maxTilesY - 50; y++)
            {
                // Don't spawn asteroids too close to the spawn area
                if (Math.Abs(x - Main.spawnTileX) < gen_spawnExclusionRadius)
                    continue;


                if (WorldGen.genRand.NextBool(100000))
                {
                    //int wallType = VariantWall.WallType<AstrolithWall>(WallSafetyType.Natural);
                    int wallType = 0;
                    BlobTileRunner(x, y, (ushort)TileType<Astrolith>(), 3..5, 2..4, 6..12, 2f, 5, wallType: (ushort)wallType);
                    if(WorldGen.genRand.NextBool(2))
                    {
                        int radius = WorldGen.genRand.Next(3,6);
                        ForEachInCircle(
                            x,
                            y,
                            radius+2,
                            (i1, j1) =>
                            {
                                if (!WorldGen.InWorld(i1, j1))
                                    return;
                                FastPlaceTile(i1, j1, (ushort)TileType<Astrolith>());
                            }
                        );
                        ushort ore = OreTypes[x%OreTypes.Count];
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
                    if(FleshMeteors){
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
        int X, Y;
        X = (int)(Main.maxTilesX / 2);
        Y = (int)(Main.maxTilesY / 2);
        Structure.Get<BaseSpaceStationModule>().Place(new(X+5, Y), null);
    }
}
