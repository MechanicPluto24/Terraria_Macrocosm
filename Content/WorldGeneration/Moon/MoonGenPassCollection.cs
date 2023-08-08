using Macrocosm.Common.Bases;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Construction;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using static Macrocosm.Common.Utils.Utility;
using Humanizer;
using static Humanizer.On;
using Macrocosm.Content.Tiles.Walls;

namespace Macrocosm.Content.WorldGeneration.Moon
{
    internal class MoonGenPassCollection : GenPassCollection
    {
        public int RegolithLayerHeight { get; } = 200;
        public float SurfaceWidthFrequency { get; } = 0.0010f;
        public float SurfaceHeightFrequency { get; } = 30f;
        public float TerrainPercentage { get; } = 0.8f;
        private int GroundY => (int)(Main.maxTilesY * (1f - TerrainPercentage));
        private static float FunnySurfaceEquation(float x) => MathF.Sin(2f * x) + MathF.Sin(MathHelper.Pi * x) + 0.4f * MathF.Cos(10f * x);
        private float? startYOffset;
        private float StartYOffset => startYOffset ??= Main.rand.NextFloat() * 2.23f;
        private int SurfaceHeight(int i) => (int)(FunnySurfaceEquation(i * SurfaceWidthFrequency + StartYOffset) * SurfaceHeightFrequency) + GroundY;

        [GenPass(InsertMode.First)]
        private void TerrainPass(GenerationProgress progress)
        {
            progress.Message = "Terrain";

            Range hallownest = 35..59;
            ushort protolithType = (ushort)TileType<Tiles.Blocks.Protolith>();
            int groundY = GroundY;

            Main.worldSurface = groundY + SurfaceHeightFrequency * 2 + 15;
            Main.rockLayer = Main.worldSurface + RegolithLayerHeight;

            PerlinNoise2D noise = new(Seed.Random);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int startJ = SurfaceHeight(i);
                for (int j = startJ; j < Main.maxTilesY; j++)
                {
                    progress.Set((float)(j + i * Main.maxTilesY) / (Main.maxTilesX * Main.maxTilesY));
                    if (
                        Main.rand.NextFloat() < 1f - 0.01f * (
                            hallownest.Start.Value + (hallownest.End.Value - hallownest.Start.Value) * MathF.Sin((float)(j - startJ) / (Main.maxTilesY - startJ) * MathHelper.Pi))
                        || noise.GetValue(i * 0.1f, j * 0.1f) > 0.15f
                        )
                    {
                        FastPlaceTile(i, j, protolithType);
                    }
                }
            }           
        }

        [GenPass(nameof(TerrainPass), InsertMode.After)]
        private void SmoothPass(GenerationProgress progress)
        {
            progress.Message = "Smoothie";

            ushort protolithType = (ushort)TileType<Tiles.Blocks.Protolith>();
            int repeats = 5;
            
            for (int x = 0; x < repeats; x++)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        progress.Set(
                            (float)(j + i * Main.maxTilesY + x * Main.maxTilesX * Main.maxTilesY) / (Main.maxTilesX * Main.maxTilesY * repeats)
                        );

                        TileNeighbourInfo neighbourInfo = new(i, j);
                        if (neighbourInfo.Solid.Count > 4)
                        {
                            FastPlaceTile(i, j, protolithType);
                            if (neighbourInfo.Solid.Count < 7 && j != 0 && j != Main.maxTilesY - 1)
                            {
                                WorldGen.SlopeTile(i, j);
                            }
                        }
                        else if (neighbourInfo.Solid.Count < 4)
                        {
                            FastRemoveTile(i, j);
                        }
                    }
                }
            }
        }

        [GenPass(nameof(SmoothPass), InsertMode.After)]
        private void RegolithPass(GenerationProgress progress)
        {
            progress.Message = "Regoliths";

            float randomOffset = Main.rand.NextFloat() * 4.23f;
            ushort regolithType = (ushort)TileType<Tiles.Blocks.Regolith>();

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(FunnySurfaceEquation(i * 0.02f + randomOffset) * 9f);
                int? maxJ = null;
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (!tile.HasTile)
                    {
                        continue;
                    }

                    if (maxJ is null)
                    {
                        maxJ = j + RegolithLayerHeight + offset;
                    }
                    else if (j >= maxJ.Value)
                    {
                        if (j % 2 == 0)
                        {
                            int veinLength = Main.rand.Next(10) switch
                            {
                                > 7 => 26,
                                _ => 8,
                            };
                            float veinEqOffset = Main.rand.NextFloat() * 12.2f;
                            int jOffset = 0;
                            while (jOffset < veinLength)
                            {
                                ForEachInCircle(
                                    i + (int)(FunnySurfaceEquation(jOffset * 0.1f + veinEqOffset) * 2f),
                                    j + jOffset,
                                    (float)jOffset / veinLength > 0.6f ? 1 : 2,
                                    (i1, j1) =>
                                    {
                                        if (CoordinatesOutOfBounds(i1, j1) || !Main.tile[i1, j1].HasTile)
                                        {
                                            return;
                                        }

                                        FastPlaceTile(i1, j1, regolithType);
                                    }
                                );

                                jOffset += 1;
                            }
                        }

                        break;
                    }

                    FastPlaceTile(i, j, regolithType);
                }
            }
        }

        [GenPass(nameof(RegolithPass), InsertMode.After)]
        private void WallPass()
        {
            int regolithWall = WallType<RegolithWall>();
            int protolithWall = WallType<ProtolithWall>();
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int startJ = SurfaceHeight(i) + 10;
                for (int j = startJ; j < Main.maxTilesY; j++)
                {
                    FastPlaceWall(i, j, j < startJ + RegolithLayerHeight ? regolithWall : protolithWall);
                }
            }
        }

        [GenPass(nameof(WallPass), InsertMode.After)]
        private void CraterPass(GenerationProgress progress)
        {
            progress.Message = "Metoeoroer";

            void SpawnMeteors(Range minMaxCount, Range minMaxRadius)
            {
                int count = Main.rand.Next(minMaxCount);
                for (int x = 0; x < count; x++)
                {
                    int i = (int)((x + Main.rand.NextFloat() * 0.9f) * (Main.maxTilesX / count));
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        if (Main.tile[i, j].HasTile)
                        {
                            int radius = Main.rand.Next(minMaxRadius);
                            ForEachInCircle(
                                i,
                                j - (int)(radius * 0.6f),
                                radius,
                                FastRemoveTile
                            );

                            break;
                        }
                    }
                }
            }

            SpawnMeteors(2..5, 100..150);
            SpawnMeteors(5..6, 30..40);
            SpawnMeteors(25..36, 7..15);
        }

        [GenPass(InsertMode.Last)]
        private void SpawnPass(GenerationProgress progress)
        {
            progress.Message = "Spawn";
            int spawnTileX = Main.maxTilesX / 2;
            Main.spawnTileX = spawnTileX;
            for (int tileY = 0; tileY < Main.maxTilesY; tileY++)
            {
                if (Main.tile[spawnTileX, tileY].HasTile)
                {
                    Main.spawnTileY = tileY - 2;
                    break;
                }
            }

            Main.dayTime = true;
            Main.time = 0;
        }
    }
}
