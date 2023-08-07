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

namespace Macrocosm.Content.WorldGeneration.Moon
{
    internal class MoonGenPassCollection : GenPassCollection
    {
        public int RegolithLayerHeight { get; } = 200;
        public float SurfaceWidthFrequency { get; } = 0.0010f;
        public float SurfaceHeightFrequency { get; } = 30f;
        public float TerrainPercentage { get; } = 0.8f;
        private int GroundY => (int)(Main.maxTilesY * (1f - TerrainPercentage));
        private float FunnySurfaceEquation(float x) => MathF.Sin(2f * x) + MathF.Sin(MathHelper.Pi * x) + 0.4f * MathF.Cos(10f * x);

        [GenPass(InsertMode.First)]
        private void TerrainPass(GenerationProgress progress)
        {
            progress.Message = "Terrain";

            Range hallownest = 35..59;
            ushort protolithType = (ushort)TileType<Tiles.Blocks.Protolith>();
            int groundY = GroundY;

            Main.worldSurface = groundY + SurfaceHeightFrequency;
            Main.rockLayer = Main.worldSurface + RegolithLayerHeight;

            PerlinNoise2D noise = new(Seed.Random);
            float startYOffset = Main.rand.NextFloat() * 2.23f;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int startJ = (int)(FunnySurfaceEquation(i * SurfaceWidthFrequency + startYOffset) * SurfaceHeightFrequency) + groundY;
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
                        /*WorldGen.PlaceTile(
                            i, 
                            j, 
                            protolithType, 
                            true, 
                            true
                        );*/
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
                    else if (j > maxJ.Value)
                    {
                        break;
                    }

                    FastPlaceTile(i, j, regolithType);
                }
            }
        }

        [GenPass(nameof(RegolithPass), InsertMode.After)]
        private void CraterPass(GenerationProgress progress)
        {
            progress.Message = "Metoeoroer";

            void SpawnMeteors(float chance, Range minMaxRadius)
            {
                for (int i = 0; i < Main.maxTilesX; i += Main.rand.Next(minMaxRadius))
                {
                    if (Main.rand.NextFloat() < chance)
                    {
                        for (int j = 0; j < Main.maxTilesY; j++)
                        {
                            if (Main.tile[i, j].HasTile)
                            {
                                int radius = Main.rand.Next(minMaxRadius);
                                ForEachInCircle(
                                    i,
                                    j - (int)(radius * 0.3f),
                                    radius,
                                    FastRemoveTile
                                );

                                break;
                            }
                        }
                    }
                }
            }

            SpawnMeteors(0.05f, 100..150);
            SpawnMeteors(0.10f, 30..40);
            SpawnMeteors(0.25f, 7..15);
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
