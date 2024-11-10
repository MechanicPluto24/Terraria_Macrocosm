using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
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
using Macrocosm.Content.Items.Tools;
using Macrocosm.Content.Items.Torches;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Furniture.Industrial;
using Macrocosm.Content.Tiles.Furniture.Luminite;
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
        public int CynthalithlithLayerHeight => 65;
        public int RegolithLayerHeight => 200;
        private float SurfaceWidthFrequency => 0.003f;
        private float SurfaceHeightFrequency => 20f;
        private float TerrainPercentage => 0.8f;
        private int SpawnFlatArea => 50;
        private int GroundY => (int)(Main.maxTilesY * (1f - TerrainPercentage));
        private float SurfaceEquation(float x) => MathF.Sin(2f * x) + MathF.Sin(MathHelper.Pi * x) + 0.4f * MathF.Cos(10f * x);
        private int SurfaceHeight(int i)
        {
            float x = i * SurfaceWidthFrequency + gen_StartYOffset;
            float normalHeight = SurfaceEquation(x) * SurfaceHeightFrequency + GroundY;

            int spawnX = Main.spawnTileX;

            int flatWidth = SpawnFlatArea * 2;
            int blendWidth = SpawnFlatArea;

            int flatStart = spawnX - flatWidth / 2;
            int flatEnd = spawnX + flatWidth / 2;

            int blendStart = flatStart - blendWidth;
            int blendEnd = flatEnd + blendWidth;

            float flatHeight = SurfaceEquation(spawnX * SurfaceWidthFrequency + gen_StartYOffset) * SurfaceHeightFrequency + GroundY;

            if (i >= flatStart && i <= flatEnd)
            {
                return (int)flatHeight;
            }
            else if (i >= blendStart && i < flatStart)
            {
                float t = (i - blendStart) / (float)(flatStart - blendStart);
                float height = MathHelper.Lerp(normalHeight, flatHeight, t);
                return (int)height;
            }
            else if (i > flatEnd && i <= blendEnd)
            {
                float t = (i - flatEnd) / (float)(blendEnd - flatEnd);
                float height = MathHelper.Lerp(flatHeight, normalHeight, t);
                return (int)height;
            }
            else
            {
                return (int)normalHeight;
            }
        }

        private bool gen_IsIrradiationRight;
        private float gen_StartYOffset;


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

            void CarveCraters(Range minMaxCount, Range minMaxRadius)
            {
                int count = WorldGen.genRand.Next(minMaxCount);
                for (int x = 0; x < count; x++)
                {
                    int i = (int)((x + WorldGen.genRand.NextFloat() * 0.9f) * (Main.maxTilesX / count));

                    if (Math.Abs(i - (Main.maxTilesX / 2)) < Math.Max(SpawnFlatArea, minMaxRadius.End.Value))
                        continue;

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

            CarveCraters(2..5, 100..150);
            CarveCraters(5..6, 30..40);
            CarveCraters(25..36, 7..15);
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

                if (Math.Abs(i - (Main.maxTilesX / 2)) < SpawnFlatArea)
                    continue;

                if (WorldGen.genRand.NextFloat() < verticalTunnelSpawnChance)
                {
                    skipI = verticalTunnelSpread;
                    int surfaceHeight = SurfaceHeight(i);
                    float eqOffset = WorldGen.genRand.NextFloat() * 10.25f;
                    float tunnelLength = verticalTunnelLength * WorldGen.genRand.NextFloat(0.45f, 1.2f);
                    float tunnelSize = verticalTunnelSize * WorldGen.genRand.NextFloat(0.6f, 1f);
                    for (int j = 0; j < tunnelLength; j += (int)(tunnelSize * 0.66f))
                    {
                        int radius = (int)(((SurfaceEquation(j * 0.01f + eqOffset * 2f) + 1f) * 0.1f + 0.8f) * tunnelSize);

                        int iPos = i + (int)(SurfaceEquation(j * 0.005f + eqOffset) * tunnelSize * 3.5f);
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
        private void RegolithTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

            float randomOffset = WorldGen.genRand.NextFloat() * 4.23f;
            ushort regolithType = (ushort)TileType<Regolith>();

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 9f);
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
                                i + (int)(SurfaceEquation(jOffset * 0.1f + veinEqOffset) * 2f),
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
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + CynthalithlithLayerHeight; j < surfaceHeight + RegolithLayerHeight + 30; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }
                    FastPlaceTile(i, j, cynthalithType);

                    float Distance = Math.Abs(surfaceHeight + CynthalithlithLayerHeight + 80 - j) / ((surfaceHeight + RegolithLayerHeight + 30) - (surfaceHeight + CynthalithlithLayerHeight) * 0.5f);
                    if (WorldGen.genRand.NextFloat() > Distance * 0.16f)
                    {
                        continue;
                    }
                    WorldGen.TileRunner(i, j, WorldGen.genRand.Next(10, 25), WorldGen.genRand.Next(10, 60), cynthalithType);
                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + CynthalithlithLayerHeight; j < surfaceHeight + RegolithLayerHeight + 30; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }
                    if (WorldGen.genRand.NextBool(300))
                    {
                        WorldGen.TileRunner(i, j, WorldGen.genRand.Next(6, 12), WorldGen.genRand.Next(6, 12), (ushort)TileType<Regolith>());
                    }

                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + CynthalithlithLayerHeight + 80; j < surfaceHeight + RegolithLayerHeight + 30; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }
                    if (WorldGen.genRand.NextBool(300))
                    {
                        WorldGen.TileRunner(i, j, WorldGen.genRand.Next(6, 12), WorldGen.genRand.Next(6, 12), (ushort)TileType<Protolith>());
                    }

                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int offset = (int)(SurfaceEquation(i * 0.02f + randomOffset) * 3f);
                int surfaceHeight = SurfaceHeight(i);
                for (int j = surfaceHeight + RegolithLayerHeight + 30; j < surfaceHeight + RegolithLayerHeight + 140; j++)
                {
                    if (!Main.tile[i, j].HasTile)
                    {
                        continue;
                    }
                    if (WorldGen.genRand.NextBool(300))
                    {
                        WorldGen.TileRunner(i, j, WorldGen.genRand.Next(6, 12), WorldGen.genRand.Next(6, 12), (ushort)TileType<Cynthalith>());
                    }

                }
            }
        }

        private static int IrradiationEquation(int x, float a, int b)
        {
            static int Equation(int x) => (int)(((10 * Math.Sin(x / 20)) * (Math.Cos(x / 5)) + ((int)(2 * Math.Sin(MathHelper.Pi * x / 80)) ^ 2)) / 1.4);
            return (int)(((a * Equation(x)) + ((x ^ 2) / a) + (Math.Abs(b * x))) / 15);
        }

        //[Task]
        private void IrradiationPass(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.IrradiationPass");
            //So many variables, but hey, this is how I do world gen. It does make it consice.
            ushort irradiationRockType = (ushort)TileType<IrradiatedRock>();
            ushort irradiationWallType = (ushort)WallType<IrradiatedRockWall>();
            int irradiationCenter = gen_IsIrradiationRight ? (int)(Main.maxTilesX / 2) + WorldGen.genRand.Next(500, 600) : (int)(Main.maxTilesX / 2) - WorldGen.genRand.Next(500, 600);
            int irradiationHeight = 200;
            int irradiationWidth = WorldGen.genRand.Next(170, 230);

            for (int radius, x = 0; x < irradiationHeight; x += 1 + (int)(radius * 0.1f))
            {

                radius = (int)((float)irradiationWidth * (1.5f - (float)x / irradiationHeight));
                int iOffset = irradiationCenter + WorldGen.genRand.NextDirection(10..50);
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
                            FastPlaceWall(i1, j1, irradiationWallType);
                        }

                        if (Main.tile[i1, j1].HasTile)
                        {
                            FastPlaceTile(i1, j1, irradiationRockType);
                        }
                    }
                );
            }

            for (int radius, x = 0; x < (irradiationHeight / 1.5); x += 1 + (int)(radius * 0.1f))
            {

                radius = (int)((float)irradiationWidth * (0.3f - (((float)x / (irradiationHeight)) * 0.3f)));
                int iOffset = irradiationCenter + WorldGen.genRand.NextDirection(10..20);
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


                        if (Main.tile[i1, j1].WallType != WallID.None || j1 > CynthalithlithLayerHeight + SurfaceHeight(i1))
                        {
                            FastPlaceWall(i1, j1, irradiationWallType);
                        }

                        if (Main.tile[i1, j1].HasTile)
                        {
                            FastRemoveTile(i1, j1);
                        }
                    }
                );
            }
            //Cave at the bottom
            for (int i = 0; i < WorldGen.genRand.Next(3, 4); i++)
            {
                WorldGen.TileRunner(irradiationCenter + WorldGen.genRand.NextDirection(4..8), (int)Main.worldSurface + irradiationHeight + WorldGen.genRand.NextDirection(4..8), WorldGen.genRand.Next(40, 80), WorldGen.genRand.Next(200, 400), -1);
            }
            for (int i = 0; i < WorldGen.genRand.Next(8, 14); i++)
            {
                WorldGen.TileRunner(irradiationCenter + WorldGen.genRand.NextDirection(30..100), (int)Main.worldSurface + WorldGen.genRand.Next(irradiationHeight), WorldGen.genRand.Next(20, 26), WorldGen.genRand.Next(7, 15), -1);
            }
            //get crystals to generate
            GenerateOre(TileID.Lead, 0.001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), irradiationRockType);//replace with uranium
        }

        [Task]
        private void OreTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.OrePass");

            int protolithType = TileType<Protolith>();
            GenerateOre(TileType<Tiles.Ores.ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<Tiles.Ores.ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<Tiles.Ores.DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<Tiles.Ores.SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);

            GenerateOre(TileID.LunarOre, 0.00005, WorldGen.genRand.Next(9, 15), WorldGen.genRand.Next(9, 15), protolithType);
        }

        //[Task]
        private void QuartzTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.OrePass");

            int tries = 0;
            int geodes = WorldGen.genRand.Next(10, 14);
            int quartzType = TileType<QuartzBlock>();
            while (geodes > 1 && tries < 100)
            {
                while (geodes > 1 && tries < 100)
                {
                    int i = WorldGen.genRand.Next(70, Main.maxTilesX - 70);
                    int j = WorldGen.genRand.Next((int)(Main.maxTilesY / 3), Main.maxTilesY - 70);
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile)
                    {
                        int iOffset2 = i;
                        int jOffset2 = j;
                        int radius = WorldGen.genRand.Next(40, 46);

                        int radius2 = WorldGen.genRand.Next(30, 36);
                        ForEachInCircle(
                            iOffset2,
                            jOffset2,
                            radius2,
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i) / (radius2 * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j) / (radius2 * 0.5f);

                                if (Main.tile[i1, j1].HasTile)
                                {
                                    FastRemoveTile(i1, j1);
                                }

                            }
                        );

                        ForEachInCircle(
                            iOffset2,
                            jOffset2,
                            radius,
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i) / (radius * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j) / (radius * 0.5f);

                                if (WorldGen.genRand.NextFloat() < 0.96f)
                                {
                                    return;
                                }
                                if (!Main.tile[i1, j1].HasTile)
                                {
                                    return;
                                }

                                TileRunnerButItDoesntIgnoreAir(i1, j1, WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 10), (ushort)quartzType);
                            }
                        );
                        ForEachInCircle(
                            iOffset2,
                            jOffset2,
                            radius,
                            (i1, j1) =>
                            {
                                if (CoordinatesOutOfBounds(i1, j1))
                                {
                                    return;
                                }

                                float iDistance = Math.Abs(iOffset2 - i) / (radius * 0.5f);
                                float jDistance = Math.Abs(jOffset2 - j) / (radius * 0.5f);

                                if (WorldGen.genRand.NextFloat() < 0.2f)
                                {
                                    return;
                                }


                                if (Main.tile[i1, j1].HasTile)
                                {
                                    FastPlaceTile(i1, j1, (ushort)quartzType);
                                }
                            }
                        );
                        geodes--;
                    }
                    else
                    {
                        tries++;
                    }
                }

            }
        }

        [Task]
        private void PlaceLuminiteShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new LuminiteShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 3.5 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 3.5 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.45f), (int)(Main.maxTilesY * 0.55f));

                bool solidDown = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(solidGround.X - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 0.8f));

                validPositionFound = solidDown && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound && shrine.Place(origin, StructureMap))
                {
                    WorldUtils.Gen(new Point(origin.X + shrine.Size.X / 2, origin.Y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: true), new CustomActions.ClearTileSafelyPostGen());
                    WorldUtils.Gen(new Point(origin.X + shrine.Size.X / 2, origin.Y + shrine.Size.Y / 2), new CustomShapes.ChasmSideways(12, 8, 80, 2, 0, dir: false), new CustomActions.ClearTileSafelyPostGen());
                }

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.LuminiteShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceHeavenforgeShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new HeavenforgeShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 0 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 0 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidDown = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(solidGround.X - shrine.Size.X / 2, solidGround.Y);

                validPositionFound = solidDown && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.HeavenforgeShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceMercuryShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new MercuryShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 1 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 1 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidUp = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Up(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(solidGround.X + shrine.Size.X / 2 - 1, solidGround.Y - 10);

                validPositionFound = solidUp && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.MercuryShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceLunarRustShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new LunarRustShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 2 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 2 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidUp = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Up(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(x + shrine.Size.X / 4, solidGround.Y - 10);

                validPositionFound = solidUp && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.LunarRustShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceStarRoyaleShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new StarRoyaleShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 3 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 3 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidDown = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(x - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 1.1f));

                validPositionFound = solidDown && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.StarRoyaleShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceCryocoreShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new CryocoreShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 4 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 4 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidDown = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(x - shrine.Size.X / 2, solidGround.Y - shrine.Size.Y - (int)(shrine.Size.Y * 0.2f));

                validPositionFound = solidDown && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.StarRoyaleShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceAstraShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new AstraShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 5 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 5 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidDown = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(x - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y));

                validPositionFound = solidDown && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.AstraShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceDarkCelestialShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new DarkCelestialShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 6 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 6 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidDown = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(x - shrine.Size.X / 2, solidGround.Y - (int)(shrine.Size.Y * 0.9f));

                validPositionFound = solidDown && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.DarkCelestialShrineUnlocked, true);
            }
        }

        [Task]
        private void PlaceCosmicEmberShrine(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            Structure shrine = new CosmicEmberShrine();
            int x, y;
            bool validPositionFound = false;
            int maxAttempts = 1000;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                x = WorldGen.genRand.Next((int)(Main.maxTilesX * (0.115f * 7 + 0.03f)), (int)(Main.maxTilesX * (0.145f * 7 + 0.03f)));
                y = WorldGen.genRand.Next((int)(Main.maxTilesY * 0.55f), (int)(Main.maxTilesY * 0.65f));

                bool solidDown = WorldUtils.Find(new(x, y), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                Point16 origin = new(x - shrine.Size.X / 2, solidGround.Y - shrine.Size.Y);

                validPositionFound = solidDown && StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, shrine.Size.X, shrine.Size.Y));

                if (validPositionFound)
                    shrine.Place(origin, StructureMap);

                attempts++;
            }

            if (!validPositionFound)
            {
                WorldFlags.SetFlag(ref WorldFlags.CosmicEmberShrineUnlocked, true);
            }
        }

        [Task]
        private void OutpostsTasks(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.StructurePass");

            int maxAttempts = 10000;
            int attempts = 0;
            int count = WorldGen.genRand.Next(10, 30);
            int placedOutposts = 0;

            while (placedOutposts < count && attempts < maxAttempts)
            {
                attempts++;

                int tileX = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int tileY = WorldGen.genRand.Next((int)(SurfaceHeight(tileX) + RegolithLayerHeight + 20.0), Main.maxTilesY - 230);

                int random = WorldGen.genRand.Next(0, 9);
                Structure outpost = random switch
                {
                    1 => new StorageOutpostSmall(),
                    2 => new StorageOutpostLarge(),
                    3 => new LabOutpost(),
                    4 => new LabOutpost2(),
                    5 => new MedicOutpost(),
                    6 => new MedicOutpost2(),
                    7 => new ServerOutpost(),
                    8 => new MiningOutpost(),
                    _ => new MiningOutpost2(),
                };

                bool solidDown = WorldUtils.Find(new Point(tileX, tileY), Searches.Chain(new Searches.Down(150), new Conditions.IsSolid()), out Point solidGround);
                if (solidDown)
                {
                    Point16 origin = new(tileX - outpost.Size.X / 2, solidGround.Y - outpost.Size.Y);

                    if (StructureMap.CanPlace(new Rectangle(origin.X, origin.Y, outpost.Size.X, outpost.Size.Y)))
                    {
                        if (outpost.Place(origin, StructureMap))
                        {
                            placedOutposts++;
                            progress.Set((double)placedOutposts / count);
                        }
                    }
                }
            }
        }


        [Task]
        private void SmoothTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SmoothPass");
            SmoothWorld(progress);
        }

        // [Task]
        private void CheeseHouse(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.Horror");
            Structure cheeseHouse = new CheeseHouse();
            cheeseHouse.Place(new Point16(420, 1000), StructureMap);
        }

        [Task]
        private void AmbientTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.AmbientPass");

            float smallRockSpawnChance = 0.1f;
            float mediumRockSpawnChance = 0.05f;
            float largeRockSpawnChance = 0.01f;
            float altarChance = 0.01f;
            float kyaniteNestChance = 0.0045f;
            float chestChance = 0.003f;

            ushort regolithType = (ushort)TileType<Regolith>();
            ushort protolithType = (ushort)TileType<Protolith>();
            ushort irradiatedRockType = (ushort)TileType<IrradiatedRock>();

            for (int i = 0; i < Main.maxTilesX - 1; i++)
            {
                progress.Set((float)i / Main.maxTilesX);

                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (WorldGen.genRand.NextFloat() < chestChance && tile.HasTile && Main.tileSolid[tile.TileType] && !tile.IsActuated)
                    {
                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<LuminiteChest>(), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < smallRockSpawnChance && tile.HasTile && Main.tileSolid[tile.TileType] && !tile.IsActuated)
                    {
                        if (tile.TileType == regolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockSmallNatural>(), style: WorldGen.genRand.Next(10), mute: true);

                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<ProtolithRockSmallNatural>(), style: WorldGen.genRand.Next(10), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < mediumRockSpawnChance && CheckEmptyAboveWithSolidToTheRight(i, j, 2, 1))
                    {
                        if (tile.TileType == regolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockMediumNatural>(), style: WorldGen.genRand.Next(6), mute: true);

                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<ProtolithRockMediumNatural>(), style: WorldGen.genRand.Next(6), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < largeRockSpawnChance && CheckEmptyAboveWithSolidToTheRight(i, j, 3, 2))
                    {
                        if (tile.TileType == regolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockLargeNatural>(), style: WorldGen.genRand.Next(5), mute: true);

                        if (tile.TileType == protolithType)
                            WorldGen.PlaceTile(i, j - 1, TileType<ProtolithRockLargeNatural>(), style: WorldGen.genRand.Next(5), mute: true);
                    }

                    if (WorldGen.genRand.NextFloat() < altarChance && CheckEmptyAboveWithSolidToTheRight(i, j, 3, 2))
                    {
                        if (tile.TileType == protolithType || tile.TileType == irradiatedRockType)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<IrradiatedAltar>(), mute: true);
                            int iOffset2 = i;
                            int jOffset2 = j;
                            int radius = WorldGen.genRand.Next(10, 16);
                            ForEachInCircle(
                                    iOffset2,
                                    jOffset2,
                                    radius,
                                    (i1, j1) =>
                                    {
                                        if (Main.tile[i, j - 1].TileType != TileType<IrradiatedAltar>())
                                        {
                                            return;
                                        }
                                        if (CoordinatesOutOfBounds(i1, j1))
                                        {
                                            return;
                                        }

                                        float iDistance = Math.Abs(iOffset2 - i) / (radius * 0.5f);
                                        float jDistance = Math.Abs(jOffset2 - j) / (radius * 0.5f);

                                        if (WorldGen.genRand.NextFloat() < 0.5f)
                                        {
                                            return;
                                        }

                                        if (Main.tile[i1, j1].HasTile && TileID.Sets.CanBeClearedDuringOreRunner[Main.tile[i1, j1].TileType] && !(Main.tile[i1, j1].TileType == TileType<IrradiatedAltar>()))
                                        {
                                            FastPlaceTile(i1, j1, (ushort)TileType<IrradiatedRock>());
                                        }
                                    }
                                );
                        }
                    }
                    if (WorldGen.genRand.NextFloat() < altarChance * 16f && CheckEmptyAboveWithSolidToTheRight(i, j, 3, 2) && j > (int)Main.worldSurface + 100)
                    {
                        if (tile.TileType == irradiatedRockType)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<IrradiatedAltar>(), mute: true);
                        }
                    }

                    if (WorldGen.genRand.NextFloat() < kyaniteNestChance && CheckEmptyAboveWithSolidToTheRight(i, j, 4, 3))
                    {
                        if (tile.TileType == protolithType)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<KyaniteNest>(), mute: true);
                        }
                    }
                }
            }
        }

        [Task]
        private void MonolithTask(GenerationProgress progress)
        {
            int tries = 0;
            bool placed = false;
            while (tries < 1000)
            {
                int tileX = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int tileY = WorldGen.genRand.Next((int)(SurfaceHeight(tileX) + RegolithLayerHeight + 20.0), Main.maxTilesY - 230);
                if (CheckEmptyAboveWithSolidToTheRight(tileX, tileY, 4, 8))
                {
                    if (WorldGen.PlaceTile(tileX, tileY - 1, TileType<Monolith>(), mute: true))
                    {
                        placed = true;
                        break;
                    }
                }
            }

            if (!placed)
            {
                int tileX = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int tileY = WorldGen.genRand.Next((int)(SurfaceHeight(tileX) + RegolithLayerHeight + 20.0), Main.maxTilesY - 230);
                WorldGen.PlaceTile(tileX, tileY - 1, TileType<Monolith>(), mute: true, forced: true);
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

        [Task]
        private void ChestLootTask(GenerationProgress progress)
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest != null)
                {
                    if (Main.tile[chest.x, chest.y].TileType == TileType<IndustrialChest>())
                    {
                        ManageIndustrialChest(chest, i);
                    }

                    if (Main.tile[chest.x, chest.y].TileType == TileType<LuminiteChest>())
                    {
                        LuminiteStyle style = (LuminiteStyle)(Main.tile[chest.x, chest.y].TileFrameX / (18 * 2 * 2));
                        ManageLuminiteChest(chest, i, style);
                    }
                }
            }
        }

        public void ManageLuminiteChest(Chest chest, int index, LuminiteStyle style)
        {
            int slot = 0;
            int random;

            if (style is LuminiteStyle.Luminite)
            {
                switch ((index % 9) + 1)
                {
                    case 1:
                        chest.item[slot++].SetDefaults(ItemType<RyuguStaff>());
                        break;
                    case 2:
                        chest.item[slot++].SetDefaults(ItemType<CrescentMoon>());
                        break;
                    case 3:
                        chest.item[slot++].SetDefaults(ItemType<ArmstrongGauntlets>());
                        break;
                    case 4:
                        chest.item[slot++].SetDefaults(ItemType<WornLunarianDagger>());
                        break;
                    case 5:
                        chest.item[slot].SetDefaults(ItemType<RocheChakram>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(50, 251);
                        break;
                    case 6:
                        chest.item[slot++].SetDefaults(ItemType<ArcaneBarnacle>());
                        break;
                    case 7:
                        chest.item[slot++].SetDefaults(ItemType<MomentumLash>());
                        break;
                    case 8:
                        chest.item[slot++].SetDefaults(ItemType<TempestuousBand>());
                        break;
                    case 9:
                        chest.item[slot++].SetDefaults(ItemType<ThaumaturgicWard>());
                        break;
                }



                random = WorldGen.genRand.Next(1, 5);
                switch (random)
                {
                    case 1:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.ArtemiteOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                        break;
                    case 2:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.ChandriumOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                        break;
                    case 3:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.DianiteOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                        break;
                    case 4:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.SeleniteOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(12, 20);
                        break;
                }
            }
            else
            {
                Chest.Lock(chest.x, chest.y);

                switch (style)
                {
                    case LuminiteStyle.Heavenforge:
                        chest.item[slot++].SetDefaults(ItemType<Procellarum>());
                        break;
                    case LuminiteStyle.Mercury:
                        chest.item[slot++].SetDefaults(ItemType<ManisolBlades>());
                        break;
                    case LuminiteStyle.LunarRust:
                        chest.item[slot++].SetDefaults(ItemType<Ilmenite>());
                        break;
                    case LuminiteStyle.StarRoyale:
                        chest.item[slot++].SetDefaults(ItemType<StarDestroyer>());
                        break;
                    case LuminiteStyle.Cryocore:
                        chest.item[slot++].SetDefaults(ItemType<FrigorianGaze>());
                        break;
                    case LuminiteStyle.Astra:
                        chest.item[slot++].SetDefaults(ItemType<Micronova>());
                        break;
                    case LuminiteStyle.DarkCelestial:
                        chest.item[slot++].SetDefaults(ItemType<Totality>());
                        break;
                    case LuminiteStyle.CosmicEmber:
                        chest.item[slot++].SetDefaults(ItemType<GreatstaffOfHorus>());
                        break;
                    default:
                        break;
                }

                if (style is LuminiteStyle.Heavenforge or LuminiteStyle.Mercury)
                {
                    chest.item[slot].SetDefaults(ItemType<ArtemiteBar>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    chest.item[slot].SetDefaults(ItemType<ArtemiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(24, 65);
                    chest.item[slot].SetDefaults(ItemID.FragmentSolar);
                    chest.item[slot++].stack = WorldGen.genRand.Next(10, 15);
                }

                if (style is LuminiteStyle.LunarRust or LuminiteStyle.StarRoyale)
                {
                    chest.item[slot].SetDefaults(ItemType<SeleniteBar>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    chest.item[slot].SetDefaults(ItemType<SeleniteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(24, 65);
                    chest.item[slot].SetDefaults(ItemID.FragmentVortex);
                    chest.item[slot++].stack = WorldGen.genRand.Next(10, 15);
                }

                if (style is LuminiteStyle.Cryocore or LuminiteStyle.Astra)
                {
                    chest.item[slot].SetDefaults(ItemType<DianiteBar>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    chest.item[slot].SetDefaults(ItemType<DianiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(24, 65);
                    chest.item[slot].SetDefaults(ItemID.FragmentNebula);
                    chest.item[slot++].stack = WorldGen.genRand.Next(10, 15);
                }

                if (style is LuminiteStyle.DarkCelestial or LuminiteStyle.CosmicEmber)
                {
                    chest.item[slot].SetDefaults(ItemType<ChandriumBar>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    chest.item[slot].SetDefaults(ItemType<ChandriumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(24, 65);
                    chest.item[slot].SetDefaults(ItemID.FragmentStardust);
                    chest.item[slot++].stack = WorldGen.genRand.Next(10, 15);
                }
            }



            if (WorldGen.genRand.NextBool())
            {
                chest.item[slot].SetDefaults(ItemID.LunarOre);
                chest.item[slot++].stack = WorldGen.genRand.Next(36, 105);
            }

            if (WorldGen.genRand.NextBool(15))
            {
                chest.item[slot++].SetDefaults(ItemType<CraterDemonSummon>());
            }

            random = WorldGen.genRand.Next(1, 3);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemType<SpaceDust>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ItemType<AlienResidue>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
                    break;
            }

            random = WorldGen.genRand.Next(1, 3);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemType<LunarCrystal>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 45);
                    break;

                case 2:
                    chest.item[slot].SetDefaults(ItemType<LuminiteTorch>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(1, 125);
                    break;
            }

            chest.item[slot].SetDefaults(ItemType<Moonstone>());
            chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
        }

        public void ManageIndustrialChest(Chest chest, int index)
        {
            int slot = 0;
            int random;

            switch ((index % 9) + 1)
            {
                case 1:
                    chest.item[slot++].SetDefaults(ItemType<ClawWrench>());
                    break;
                case 2:
                    chest.item[slot++].SetDefaults(ItemType<StopSign>());
                    chest.item[slot++].SetDefaults(ItemType<EmployeeVisor>());
                    chest.item[slot++].SetDefaults(ItemType<EmployeeSuit>());
                    chest.item[slot++].SetDefaults(ItemType<EmployeeBoots>());
                    break;
                case 3:
                    chest.item[slot++].SetDefaults(ItemType<WaveGunRed>());
                    break;
                case 4:
                    chest.item[slot++].SetDefaults(ItemType<Copernicus>());
                    break;
                case 5:
                    chest.item[slot++].SetDefaults(ItemType<HummingbirdDroneRemote>());
                    break;
                case 6:
                    chest.item[slot++].SetDefaults(ItemType<OsmiumBoots>());
                    break;
                case 7:
                    chest.item[slot++].SetDefaults(ItemType<StalwartTowerShield>());
                    break;
                case 8:
                    chest.item[slot++].SetDefaults(ItemType<Sledgehammer>());
                    break;
                case 9:
                    chest.item[slot++].SetDefaults(ItemType<LaserSight>());
                    break;
            }

            random = WorldGen.genRand.Next(1, 3);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemType<Medkit>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(5, 16);
                    break;

                case 2:
                    chest.item[slot].SetDefaults(ItemType<AntiRadiationPills>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(5, 16);
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                random = WorldGen.genRand.Next(1, 8);
                switch (random)
                {
                    case 1:
                        chest.item[slot].SetDefaults(ItemType<Plastic>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 2:
                        chest.item[slot].SetDefaults(ItemType<NickelOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 3:
                        chest.item[slot].SetDefaults(ItemID.LunarOre);
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 4:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.LithiumOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 5:
                        chest.item[slot].SetDefaults(ItemType<Items.Ores.AluminumOre>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 6:
                        chest.item[slot].SetDefaults(ItemType<SteelBar>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                    case 7:
                        chest.item[slot].SetDefaults(ItemType<RocketFuelCanister>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(1, 30);
                        break;
                }
            }

            random = WorldGen.genRand.Next(1, 5);
            switch (random)
            {
                case 1:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.ArtemiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 2:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.ChandriumOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 3:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.DianiteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;
                case 4:
                    chest.item[slot].SetDefaults(ItemType<Items.Ores.SeleniteOre>());
                    chest.item[slot++].stack = WorldGen.genRand.Next(12, 45);
                    break;

            }

            for (int i = 0; i < 2; i++)
            {
                random = WorldGen.genRand.Next(1, 5);
                switch (random)
                {
                    case 1:
                        chest.item[slot].SetDefaults(ItemType<Items.Blocks.IndustrialPlating>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                        break;
                    case 2:
                        chest.item[slot].SetDefaults(ItemType<Items.Blocks.Terrain.Protolith>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                        break;
                    case 3:
                        chest.item[slot].SetDefaults(ItemType<Items.Blocks.Terrain.Regolith>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                        break;
                    case 4:
                        chest.item[slot].SetDefaults(ItemType<Items.Blocks.Terrain.Cynthalith>());
                        chest.item[slot++].stack = WorldGen.genRand.Next(30, 90);
                        break;

                }
            }

            chest.item[slot].SetDefaults(ItemType<LunarCrystal>());
            chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);

            chest.item[slot].SetDefaults(ItemType<Moonstone>());
            chest.item[slot++].stack = WorldGen.genRand.Next(1, 20);
        }
    }
}