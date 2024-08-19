using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Ores;
using Macrocosm.Content.Tiles.Walls;
using Macrocosm.Content.WorldGeneration.Structures;
using Macrocosm.Content.WorldGeneration.Structures.LunarOutposts;
using Macrocosm.Content.WorldGeneration.Structures.Shrines;
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

namespace Macrocosm.Content.Subworlds
{
    public partial class Moon
    {
        public int CynthalithlithLayerHeight { get; } = 50;
        public int RegolithLayerHeight { get; } = 200;
        private float SurfaceWidthFrequency { get; } = 0.003f;
        private float SurfaceHeightFrequency { get; } = 20f;
        private float TerrainPercentage { get; } = 0.8f;
        private int GroundY => (int)(Main.maxTilesY * (1f - TerrainPercentage));
        private static float FunnySurfaceEquation(float x) => MathF.Sin(2f * x) + MathF.Sin(MathHelper.Pi * x) + 0.4f * MathF.Cos(10f * x);
        private int SurfaceHeight(int i) => (int)(FunnySurfaceEquation(i * SurfaceWidthFrequency + gen_StartYOffset) * SurfaceHeightFrequency) + GroundY;
        private static int WhatTheHellIsThisEquation(int x) => (int)(((10 * Math.Sin(x / 20)) * (Math.Cos(x / 5)) + ((int)(2 * Math.Sin(MathHelper.Pi * x / 80)) ^ 2)) / 1.4);
        private int IDontEvenHaveANameForThis(int x, float a, int b) => (int)(((a * WhatTheHellIsThisEquation(x)) + ((x ^ 2) / a) + (Math.Abs(b * x))) / 15);

        private bool gen_IsIrradiationRight;
        private static float gen_StartYOffset;

        private static Point gen_HeavenforgeShrinePosition;
        private static Point gen_MercuryShrinePosition;
        private static Point gen_LunarRustShrinePosition;
        private static Point gen_StarRoyaleShrinePosition;
        private static Point gen_AstraShrinePosition;
        private static Point gen_CryocoreShrinePosition;
        private static Point gen_DarkCelestialShrinePosition;
        private static Point gen_CosmicEmberShrinePosition;

        private Structure DetermineLunarHouse()
        {
            int i = Main.rand.Next(0, 9);
            return i switch
            {
                0 => new LunarHouse1(),
                1 => new LunarHouse2(),
                2 => new LunarHouse3(),
                3 => new LunarHouse4(),
                4 => new LunarHouse5(),
                5 => new LunarHouse6(),
                6 => new LunarHouse7(),
                7 => new LunarHouse8(),
                8 => new LunarHouse9(),
                _ => null,
            };
        }

        [Task]
        private void PrepareTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.TerrainPass");

            gen_IsIrradiationRight = WorldGen.genRand.NextBool();
        }

        [Task]
        private void TerrainTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.TerrainPass");

            ushort protolithType = (ushort)TileType<Protolith>();

            Main.worldSurface = GroundY + SurfaceHeightFrequency * 2;
            Main.rockLayer = GroundY + RegolithLayerHeight;

            gen_StartYOffset = WorldGen.genRand.NextFloat() * 2.3f;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int startJ = SurfaceHeight(i);
                for (int j = startJ; j < Main.maxTilesY; j++)
                {
                    progress.Set((float)(j + i * Main.maxTilesY) / (Main.maxTilesX * Main.maxTilesY));
                    FastPlaceTile(i, j, protolithType);
                }
            }

            int regolithWall = WallType<RegolithWall>();
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set(0.5d * i / Main.maxTilesX);
                int wallPlaceStart = SurfaceHeight(i) + 15;
                for (int j = wallPlaceStart; j < wallPlaceStart + RegolithLayerHeight; j++)
                {
                    FastPlaceWall(i, j, regolithWall);
                }
            }
        }

        [Task]
        private void CraterTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CraterPass");

            void SpawnMeteors(Range minMaxCount, Range minMaxRadius)
            {
                int count = WorldGen.genRand.Next(minMaxCount);
                for (int x = 0; x < count; x++)
                {
                    int i = (int)((x + WorldGen.genRand.NextFloat() * 0.9f) * (Main.maxTilesX / count));
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        if (Main.tile[i, j].HasTile)
                        {
                            int radius = WorldGen.genRand.Next(minMaxRadius);
                            ForEachInCircle(
                                i,
                                j - (int)(radius * 0.6f),
                                radius,
                                (i1, j1) =>
                                {
                                    FastRemoveWall(i1, j1);
                                    float iDistance = (float)Math.Abs(i - i1) / radius;
                                    float jDistance = (float)Math.Abs(j - j1) / radius;
                                    FastRemoveTile(i1, j1);
                                }
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

        [Task]
        private void CaveTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            double smallCaveFreq = 0.0009;
            double largeCaveFreq = 0.00013;

            int airTileType = -1;

            List<Point> smallCaves = new();
            List<Point> largeCaves = new();

            // generate small caves in the protolith layer 
            for (int smallCaveSpot = 0; smallCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * smallCaveFreq); smallCaveSpot++)
            {
                float percentDone = (float)((double)smallCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * smallCaveFreq));
                progress.Set(percentDone * 0.5f);

                int tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                int tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);

                // really small holes 
                WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 20), airTileType);

                tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
                while (((double)tileY < Main.rockLayer) || ((double)tileX > (double)Main.maxTilesX * 0.45 && (double)tileX < (double)Main.maxTilesX * 0.55 && (double)tileY < Main.rockLayer))
                {
                    tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                    tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
                }

                // small caves 
                smallCaves.Add(new Point(tileX, tileY));
                WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), airTileType);
            }

            //generate large caves 
            for (int largeCaveSpot = 0; largeCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * largeCaveFreq); largeCaveSpot++)
            {
                float percentDone = (float)((double)largeCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * largeCaveFreq));
                progress.Set(0.5f + percentDone * 0.5f);

                int tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
                int tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
                largeCaves.Add(new Point(tileX, tileY));
                WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(5, 26), WorldGen.genRand.Next(50, 350), airTileType);
            }
        }

        [Task]
        private void SurfaceTunnelTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            float verticalTunnelSpawnChance = 0.005f;
            int verticalTunnelSpread = 230;
            int verticalTunnelLength = RegolithLayerHeight + 170;
            int verticalTunnelSize = 11;

            int skipI = 0;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set((float)i / Main.maxTilesX);

                if (skipI > 0)
                {
                    skipI--;
                    continue;
                }

                if (WorldGen.genRand.NextFloat() < verticalTunnelSpawnChance)
                {
                    skipI = verticalTunnelSpread;
                    int surfaceHeight = SurfaceHeight(i);
                    float eqOffset = WorldGen.genRand.NextFloat() * 10.25f;
                    float tunnelLenght = verticalTunnelLength * WorldGen.genRand.NextFloat(0.45f, 1.2f);
                    float tunnelSize = verticalTunnelSize * WorldGen.genRand.NextFloat(0.6f, 1f);
                    for (int j = 0; j < tunnelLenght; j += (int)(tunnelSize * 0.66f))
                    {
                        int radius = (int)(((FunnySurfaceEquation(j * 0.01f + eqOffset * 2f) + 1f) * 0.1f + 0.8f) * tunnelSize);

                        int iPos = i + (int)(FunnySurfaceEquation(j * 0.005f + eqOffset) * tunnelSize * 3.5f);
                        int jPos = surfaceHeight + j;
                        ForEachInCircle(
                            iPos,
                            jPos,
                            radius,
                            FastRemoveTile
                        );
                    }
                }
            }
        }

        [Task]
        private void WallTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.WallPass");
            int protolithWall = WallType<ProtolithWall>();

            int wallCount = 100;
            for (int z = 0; z < wallCount; z++)
            {
                progress.Set(0.5d + 0.5d * z / wallCount);
                Point point = new();

                do
                {
                    int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                    point = new Point(x, WorldGen.genRand.Next(SurfaceHeight(x) + 15 + RegolithLayerHeight, Main.maxTilesY));
                }
                while (
                    Main.tile[point.X, point.Y].HasTile ||
                    Main.tile[point.X, point.Y].WallType == protolithWall
                );

                if (ConnectedTiles(point.X, point.Y, tile => !tile.HasTile, out var coordinates, 9999))
                {
                    foreach ((int i, int j) in coordinates)
                    {
                        FastPlaceWall(i, j, protolithWall);
                    }
                }
            }
        }

        [Task]
        private void PrepareHeavenforgeShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new HeavenforgeShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 0 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 0 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: false), new Actions.ClearTile());

            gen_HeavenforgeShrinePosition = new(x + shrine.Size.X / 2, y + shrine.Size.Y / 2);
        }

        [Task]
        private void PrepareMercuryShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new MercuryShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 1 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 1 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(30, 1, 10, 2, 0, dir: false), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(30, 10, 140, 2, 0, dir: true), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));

            gen_MercuryShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareLunarRustShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new LunarRustShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 2 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 2 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(65, 1, 10, 2, 0, dir: false), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y), new CustomShapes.Chasm(65, 10, 180, 2, 0, dir: true), Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall()));

            gen_LunarRustShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareStarRoyaleShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new StarRoyaleShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 3 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 3 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: false), new Actions.ClearTile());
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2, shrine.Size.X * 1.2, 2, -1);

            gen_StarRoyaleShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareCryocoreShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new CryocoreShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 4 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 4 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            //BlobTileRunner(, -1, 1..2, 1..10, (shrine.Size.X-1)..(), 1, 2);
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1, shrine.Size.X * 1.2, 1, -1);

            gen_CryocoreShrinePosition = new(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1);

        }

        [Task]
        private void PrepareAstraShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new AstraShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 5 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 5 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(18, 16, 50, 2, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(18, 16, 50, 2, 0, dir: false), new Actions.ClearTile());
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1, shrine.Size.X * 1.25f, 1, -1);

            gen_AstraShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareDarkCelestialShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new DarkCelestialShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 6 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 6 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: true), new Actions.ClearTile());
            WorldUtils.Gen(new Point(x + shrine.Size.X / 2, y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(30, 10, 100, 3, 0, dir: false), new Actions.ClearTile());
            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2, shrine.Size.X * 1.2, 3, -1);

            gen_DarkCelestialShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void PrepareCosmicEmberShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            Structure shrine = new CosmicEmberShrine();

            int x, y;
            do
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 7 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 7 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));
            } while (!(StructureMap.CanPlace(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y)) && WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Rectangle(shrine.Size.X, shrine.Size.Y), new Conditions.IsSolid()), out _)));
            StructureMap.AddProtectedStructure(new Rectangle(x, y, shrine.Size.X, shrine.Size.Y), 10);

            WorldGen.TileRunner(x + shrine.Size.X / 2, y + shrine.Size.Y / 2 - 1, shrine.Size.X * 2f, 1, -1);

            gen_CosmicEmberShrinePosition = new(x + shrine.Size.X / 2, y);
        }

        [Task]
        private void RegolithTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            float randomOffset = WorldGen.genRand.NextFloat() * 4.23f;
            ushort regolithType = (ushort)TileType<Regolith>();

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(FunnySurfaceEquation(i * 0.02f + randomOffset) * 9f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight; j < surfaceHeight + RegolithLayerHeight; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }

                    if (j == surfaceHeight + RegolithLayerHeight - 1 && j % 2 == 0)
                    {
                        int veinLength = WorldGen.genRand.Next(10) switch
                        {
                            > 7 => 26,
                            _ => 8,
                        };
                        float veinEqOffset = WorldGen.genRand.NextFloat() * 12.2f;
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

                    FastPlaceTile(i, j, regolithType);
                }
            }
        }
        [Task]
        private void CynthalithTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.Cynthalith");

            float randomOffset = WorldGen.genRand.NextFloat() * 4.23f;
            ushort cynthalithType = (ushort)TileType<Cynthalith>();

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(FunnySurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + CynthalithlithLayerHeight; j < surfaceHeight + RegolithLayerHeight + 30; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }
                    if (j < surfaceHeight + 60 && Main.rand.Next(Math.Abs(surfaceHeight + 60 - j)) < 10)
                    {
                        continue;
                    }
                    FastPlaceTile(i, j, cynthalithType);
                }
            }
        }

        //[Task]
        private void IrradiationPass(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.IrradiationPass");
            //So many variables, but hey, this is how I do world gen. It does make it consice.
            ushort IrradiationRockType = (ushort)TileType<IrradiatedRock>();
            ushort IrradiationWallType = (ushort)WallType<IrradiatedRockWall>();
            int IrradiationCenter = gen_IsIrradiationRight ? (int)(Main.maxTilesX / 2) + Main.rand.Next(500, 600) : (int)(Main.maxTilesX / 2) - Main.rand.Next(500, 600);
            int IrradiationHeight = 400;
            int IrradiationWidth = Main.rand.Next(240, 260);

            //reused this because eh, im bad at smooth tile conversion.
            for (int radius, x = 0; x < IrradiationHeight; x += 1 + (int)(radius * 0.1f))
            {

                radius = (int)((float)IrradiationWidth * (1.5f - (float)x / IrradiationHeight));
                int iOffset = IrradiationCenter + WorldGen.genRand.NextDirection(10..50);
                int jOffset = (int)Main.worldSurface + x + WorldGen.genRand.NextDirection(20..30);
                ForEachInCircle(
                    iOffset,
                    jOffset,
                    radius,
                    (i1, j1) =>
                    {
                        if (CoordinatesOutOfBounds(i1, j1))
                        {
                            return;
                        }

                        float iDistance = Math.Abs(iOffset - i1) / (radius * 0.5f);
                        float jDistance = Math.Abs(jOffset - j1) / (radius * 0.5f);
                        if (WorldGen.genRand.NextFloat() < iDistance * 0.2f || WorldGen.genRand.NextFloat() < jDistance * 0.2f)
                        {
                            return;
                        }

                        if (Main.tile[i1, j1].WallType != WallID.None)
                        {
                            FastPlaceWall(i1, j1, IrradiationWallType);
                        }

                        if (Main.tile[i1, j1].HasTile)
                        {
                            FastPlaceTile(i1, j1, IrradiationRockType);
                        }
                    }
                );
            }

            //basically do the same thing but make it smaller and remove tiles.
            for (int radius, x = 0; x < (IrradiationHeight / 1.5); x += 1 + (int)(radius * 0.1f))
            {

                radius = (int)((float)IrradiationWidth * (0.3f - (((float)x / (IrradiationHeight)) * 0.3f)));
                int iOffset = IrradiationCenter + WorldGen.genRand.NextDirection(10..20);
                int jOffset = (int)Main.worldSurface + x + WorldGen.genRand.NextDirection(20..30);
                ForEachInCircle(
                    iOffset,
                    jOffset,
                    radius,
                    (i1, j1) =>
                    {
                        if (CoordinatesOutOfBounds(i1, j1))
                        {
                            return;
                        }

                        float iDistance = Math.Abs(iOffset - i1) / (radius * 0.5f);
                        float jDistance = Math.Abs(jOffset - j1) / (radius * 0.5f);





                        if (Main.tile[i1, j1].WallType != WallID.None || j1 > CynthalithlithLayerHeight + SurfaceHeight(i1))
                        {
                            FastPlaceWall(i1, j1, IrradiationWallType);
                        }

                        if (Main.tile[i1, j1].HasTile)
                        {
                            FastRemoveTile(i1, j1);
                        }
                    }
                );
            }
            //Now we create the cavern at the bottom. Done in 2 steps.
            int radius2 = (int)((float)IrradiationWidth * 0.5f);
            int iOffset2 = IrradiationCenter + WorldGen.genRand.NextDirection(2..4);
            int jOffset2 = (int)Main.worldSurface + (int)(IrradiationHeight / 1.5) - WorldGen.genRand.NextDirection(20..30);

            //Tunnels


            //Right facing tunnels
            for (int iteration = 0; iteration < Main.rand.Next(2, 5); iteration++)
            {

                float a = Main.rand.NextFloat(1.0f, 20.0f);
                int b = Main.rand.Next(0, 22);
                int AAAAAAAA = 0;//DONT ASK, THIS IS DRIVING ME MAD.

                AAAAAAAA = 0;
                for (int something = 0; something < Main.rand.Next(100, 150); something++)
                {
                    ForEachInCircle(
                            iOffset2 + AAAAAAAA,
                            jOffset2 + IDontEvenHaveANameForThis(AAAAAAAA, a, b),
                            Main.rand.Next(5, 8),
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i1) / (radius2 * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j1) / (radius2 * 0.5f);




                                FastRemoveTile(i1, j1);
                                FastPlaceWall(i1, j1, IrradiationWallType);
                            }
                        );
                    AAAAAAAA++;
                }

            }
            //left facing tunnels
            for (int iteration = 0; iteration < Main.rand.Next(2, 5); iteration++)
            {

                float a = Main.rand.NextFloat(1.0f, 20.0f);
                int b = Main.rand.Next(0, 22);
                int AAAAAAAA = 0;//DONT ASK, THIS IS DRIVING ME MAD.

                AAAAAAAA = 0;
                for (int something = 0; something < Main.rand.Next(100, 150); something++)
                {
                    ForEachInCircle(
                            iOffset2 + AAAAAAAA,
                            jOffset2 + IDontEvenHaveANameForThis(AAAAAAAA, a, b),
                            Main.rand.Next(5, 8),
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i1) / (radius2 * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j1) / (radius2 * 0.5f);




                                FastRemoveTile(i1, j1);
                                FastPlaceWall(i1, j1, IrradiationWallType);
                            }
                        );
                    AAAAAAAA--;
                }

            }
        }

        [Task]
        private void OreTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.OrePass");

            int protolithType = TileType<Protolith>();
            GenerateOre(TileType<ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);

            GenerateOre(TileID.LunarOre, 0.00005, WorldGen.genRand.Next(9, 15), WorldGen.genRand.Next(9, 15), protolithType);
        }

        [Task]
        private void SmoothTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SmoothPass");
            SmoothWorld(progress);
        }


        [Task]
        private void PlaceHeavenforgeShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new HeavenforgeShrine();
            bool solidDown = WorldUtils.Find(gen_HeavenforgeShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(solidGround.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 0.8f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void PlaceMercuryShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new MercuryShrine();
            bool solidUp = WorldUtils.Find(gen_MercuryShrinePosition, Searches.Chain(new Searches.Up(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidUp && shrine.Place(new Point16(solidGround.X + shrine.Size.X / 2 - 1, solidGround.Y - 10), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void PlaceLunarRustShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new LunarRustShrine();
            bool solidUp = WorldUtils.Find(gen_LunarRustShrinePosition, Searches.Chain(new Searches.Up(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidUp && shrine.Place(new Point16(gen_LunarRustShrinePosition.X + shrine.Size.X / 4, solidGround.Y - 10), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void PlaceStarRoyaleShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new StarRoyaleShrine();
            bool solidDown = WorldUtils.Find(gen_StarRoyaleShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_StarRoyaleShrinePosition.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 1.1f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void PlaceCryocoreShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new CryocoreShrine();
            bool solidDown = WorldUtils.Find(gen_CryocoreShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_CryocoreShrinePosition.X - shrine.Size.X / 2, solidGround.Y - shrine.Size.Y - (int)(shrine.Size.Y * 0.2f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void PlaceAstraShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new AstraShrine();
            bool solidDown = WorldUtils.Find(gen_AstraShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_AstraShrinePosition.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void PlaceDarkCelestialShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new DarkCelestialShrine();
            bool solidDown = WorldUtils.Find(gen_DarkCelestialShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_DarkCelestialShrinePosition.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 0.9f)), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void PlaceCosmicEmberShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new CosmicEmberShrine();
            bool solidDown = WorldUtils.Find(gen_CosmicEmberShrinePosition, Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
            if (solidDown && shrine.Place(new Point16(gen_CosmicEmberShrinePosition.X - shrine.Size.X / 2, solidGround.Y - shrine.Size.Y), null))
                return;

            int fallbackX = WorldGen.genRand.Next(0, Main.maxTilesX - shrine.Size.X);
            int fallbackY = WorldGen.genRand.Next(SurfaceHeight(fallbackX) + RegolithLayerHeight, Main.maxTilesY - shrine.Size.Y * 2);
            shrine.Place(new Point16(fallbackX, fallbackY), null);
        }

        [Task]
        private void RoomsTasks(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            int tries = 10000;
            int count = WorldGen.genRand.Next(20, 80);
            for (int i = 0; i < count; i++)
            {
                if (tries <= 0)
                    break;

                progress.Set((double)i / count);
                int tileX = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int tileY = WorldGen.genRand.Next((int)(SurfaceHeight(tileX) + RegolithLayerHeight + 20.0), Main.maxTilesY - 230);

                var builder = DetermineLunarHouse();
                if (!builder.Place(new(tileX, tileY), StructureMap))
                {
                    tries--;
                    i--;
                }
            }
        }

        [Task]
        private void CheeseHouse(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.Horror");
            Structure cheeseHouse = new CheeseHouse();
            cheeseHouse.Place(new Point16(420, 1000), StructureMap);
        }
        [Task]
        private void Monlith(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.Horror");
            var monolith = new MonolithStructure();
            for (int i = 0; i < 100; i++)
            {
                int x = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int y = SurfaceHeight(x);
                if (Main.tile[x, y].HasTile)
                {
                    monolith.Place(new Point16(x, y - monolith.Size.Y), StructureMap);
                    break;
                }
            }
        }

        [Task]
        private void AmbientTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.AmbientPass");
            float smallRockSpawnChance = 0.1f;
            float mediumRockSpawnChance = 0.05f;
            float largeRockSpawnChance = 0.01f;
            ushort regolithType = (ushort)TileType<Regolith>();
            float AltarChance = 0.0025f;
            ushort ProtolithType = (ushort)TileType<Protolith>();

            for (int i = 0; i < Main.maxTilesX - 1; i++)
            {
                progress.Set((float)i / Main.maxTilesX);
                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    TileNeighbourInfo neighbourInfo = new(i, j);
                    TileNeighbourInfo aboveNeighbourInfo = new(i, j - 1);
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == regolithType)
                    {
                        if (WorldGen.genRand.NextFloat() < smallRockSpawnChance)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockSmallNatural>(), style: WorldGen.genRand.Next(10), mute: true);
                        }
                        else if (
                                neighbourInfo.Solid.Right
                            && !neighbourInfo.HasTile.Top
                            && !neighbourInfo.HasTile.TopRight
                            && WorldGen.genRand.NextFloat() < mediumRockSpawnChance
                            )
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockMediumNatural>(), style: WorldGen.genRand.Next(6), mute: true);
                        }
                        else if (
                                neighbourInfo.Solid.Right
                            && neighbourInfo.Solid.Left
                            && !neighbourInfo.HasTile.Top
                            && !neighbourInfo.HasTile.TopRight
                            && !neighbourInfo.HasTile.TopLeft
                            && !aboveNeighbourInfo.HasTile.Top
                            && !aboveNeighbourInfo.HasTile.TopRight
                            && !aboveNeighbourInfo.HasTile.TopLeft
                            && WorldGen.genRand.NextFloat() < largeRockSpawnChance
                            )
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockLargeNatural>(), style: WorldGen.genRand.Next(5), mute: true);
                        }
                    }
                }
            }
            for (int i = 0; i < Main.maxTilesX - 1; i++)
            {
                progress.Set((float)i / Main.maxTilesX);
                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    TileNeighbourInfo neighbourInfo = new(i, j);
                    TileNeighbourInfo aboveNeighbourInfo = new(i, j - 1);
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ProtolithType)
                    {
                        if (neighbourInfo.Solid.Right
                            && neighbourInfo.Solid.Left
                            && !neighbourInfo.HasTile.Top
                            && !neighbourInfo.HasTile.TopRight
                            && !neighbourInfo.HasTile.TopLeft
                            && !aboveNeighbourInfo.HasTile.Top
                            && !aboveNeighbourInfo.HasTile.TopRight
                            && !aboveNeighbourInfo.HasTile.TopLeft
                            && (WorldGen.genRand.NextFloat() < AltarChance || (Main.tile[i, j].TileType == (ushort)TileType<IrradiatedRock>() && WorldGen.genRand.NextFloat() < 0.035f && j > SurfaceHeight(i) + RegolithLayerHeight)))
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<IrradiatedAltar>(), mute: true);
                        }
                    }
                }
            }
        }

        [Task]
        private void SpawnTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SpawnPass");
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
