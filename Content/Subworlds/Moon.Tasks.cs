using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Ores;
using Macrocosm.Content.Tiles.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds
{
    public partial class Moon
    {
        public int RegolithLayerHeight { get; } = 200;
        private float SurfaceWidthFrequency { get; } = 0.003f;
        private float SurfaceHeightFrequency { get; } = 20f;
        private float TerrainPercentage { get; } = 0.8f;
        private int GroundY => (int)(Main.maxTilesY * (1f - TerrainPercentage));
        private static float FunnySurfaceEquation(float x) => MathF.Sin(2f * x) + MathF.Sin(MathHelper.Pi * x) + 0.4f * MathF.Cos(10f * x);
        private static float StartYOffset { get; set; }
        private int SurfaceHeight(int i) => (int)(FunnySurfaceEquation(i * SurfaceWidthFrequency + StartYOffset) * SurfaceHeightFrequency) + GroundY;

        [Task]
        private void TerrainTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.TerrainPass");

            Range hallownest = 35..55;
            ushort protolithType = (ushort)TileType<Protolith>();

            Main.worldSurface = GroundY + SurfaceHeightFrequency * 2;
            Main.rockLayer = GroundY + RegolithLayerHeight;

            PerlinNoise2D noise = new(MacrocosmWorld.Seed);
            StartYOffset = WorldGen.genRand.NextFloat() * 2.3f;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int startJ = SurfaceHeight(i);
                for (int j = startJ; j < Main.maxTilesY; j++)
                {
                    progress.Set((float)(j + i * Main.maxTilesY) / (Main.maxTilesX * Main.maxTilesY));
                    if (
                        WorldGen.genRand.NextFloat() < 1f - 0.01f * (
                            hallownest.Start.Value + (hallownest.End.Value - hallownest.Start.Value) * MathF.Sin((float)(j - startJ) / (Main.maxTilesY - startJ) * MathHelper.Pi))
                        || noise.GetValue(i * 0.048f, j * 0.048f) > 0f
                        )
                    {
                        FastPlaceTile(i, j, protolithType);
                    }
                }
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                if (WorldGen.genRand.NextFloat() < 0.01f)
                {
                    int j = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 250);
                    ForEachInCircle(
                        i,
                        j,
                        WorldGen.genRand.Next(20, 55),
                        (i, j) =>
                        {
                            if (WorldGen.genRand.NextFloat() < 0.4f)
                            {
                                FastRemoveTile(i, j);
                            }
                        }
                    );
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
                                    if (WorldGen.genRand.NextFloat() < iDistance * 0.6f || WorldGen.genRand.NextFloat() < jDistance * 0.6f)
                                    {
                                        return;
                                    }

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
        private void SurfaceTunnelTask(GenerationProgress progress)
        {
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
                        /*BlobTileRunner(
                            i + (int)(FunnySurfaceEquation(j * 0.05f + eqOffset) * tunnelSize * 2f), 
                            surfaceHeight + j, 
                            -1,
                            6..8,
                            (int)(0.5f * radius)..(int)(0.66f * radius), 
                            radius..(radius + 5)
                        );*/
                        int iPos = i + (int)(FunnySurfaceEquation(j * 0.005f + eqOffset) * tunnelSize * 3.5f);
                        int jPos = surfaceHeight + j;
                        ForEachInCircle(
                            iPos,
                            jPos,
                            radius,
                            (i, j) =>
                            {
                                if (WorldGen.genRand.NextFloat() < 0.7f)
                                {
                                    FastRemoveTile(i, j);
                                }
                            }
                        );

                        if (WorldGen.genRand.NextFloat() < 0.1f)
                        {
                            Vector2 randomDirection = WorldGen.genRand.NextVector2Unit();
                            float offset = 17f * WorldGen.genRand.NextFloat(0.7f, 1f);
                            ForEachInCircle(
                                iPos + (int)(randomDirection.X * offset),
                                jPos + (int)(randomDirection.Y * offset),
                                radius * 2,
                                (i, j) =>
                                {
                                    if (WorldGen.genRand.NextFloat() < 0.45f)
                                    {
                                        FastRemoveTile(i, j);
                                    }
                                }
                            );
                        }
                    }
                }
            }
        }

        [Task]
        private void SmoothTask(GenerationProgress progress)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.SmoothPass");

            ushort protolithType = (ushort)TileType<Protolith>();
            int repeats = 5;

            for (int x = 0; x < repeats; x++)
            {
                ForEachInRectangle(
                    0,
                    0,
                    Main.maxTilesX,
                    Main.maxTilesY,
                    (i, j) =>
                    {
                        progress.Set(
                                0.25d * (j + i * Main.maxTilesY + x * Main.maxTilesX * Main.maxTilesY) / (Main.maxTilesX * Main.maxTilesY * repeats)
                            );

                        TileNeighbourInfo neighbourInfo = new(i, j);
                        if (neighbourInfo.Solid.Count > 4)
                        {
                            FastPlaceTile(i, j, protolithType);
                        }
                        else if (neighbourInfo.Solid.Count < 4)
                        {
                            FastRemoveTile(i, j);
                        }
                    }
                );
            }

            ForEachInRectangle(
                0,
                0,
                Main.maxTilesX,
                Main.maxTilesY,
                (i, j) =>
                {
                    progress.Set(0.25d + 0.25d * i / Main.maxTilesX);
                    if (!WorldGen.genRand.NextBool(4))
                    {
                        if (WorldGen.genRand.NextBool())
                        {
                            SafeSlopeTile(i, j);
                        }
                        else
                        {
                            SafePoundTile(i, j);
                        }
                    }
                }
            );

            stopwatch.Stop();
            Macrocosm.Instance.Logger.Info($"Smoothing time: {stopwatch.Elapsed}");

            stopwatch.Start();

            /*
            ForEachInRectangle(
                0,
                GroundY + RegolithLayerHeight,
                Main.maxTilesX,
                Main.maxTilesY,
                (i, j) =>
                {
                    progress.Set(0.5d + 0.25d * i / Main.maxTilesX);
                    if (ConnectedTiles(i, j, tile => tile.HasTile, out List<(int, int)> coordinates, 140))
                    {
                        foreach ((int x, int y) in coordinates)
                        {
                            FastRemoveTile(x, y);
                        }
                    }
                },
                2,
                2
            );
            */

            ForEachInRectangle(
                0,
                GroundY + RegolithLayerHeight,
                Main.maxTilesX,
                Main.maxTilesY,
                (i, j) =>
                {
                    progress.Set(0.75d + 0.25d * i / Main.maxTilesX);
                    var info = new TileNeighbourInfo(i, j).HasTile;
                    if (Main.tile[i, j].HasTile && info.Count == 0)
                    {
                        FastRemoveTile(i, j);
                    }
                    else if (info.Count > 6)
                    {
                        FastPlaceTile(i, j, protolithType);
                    }
                }
            );

            stopwatch.Stop();
            Macrocosm.Instance.Logger.Info($"Smoothing cleanup time: {stopwatch.Elapsed}");
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
            progress.Message = "Regoliths";

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

        //[Task]
        private void IrradiationPass(GenerationProgress progress)
        {
            progress.Message = "Piss";

            ushort irradiationRockType = (ushort)TileType<IrradiatedRock>();
            ushort irradiationWallType = (ushort)WallType<IrradiatedRockWall>();
            Range irradiationSpawnRange = 20..80;
            int irradiationHeight = 550;

            int i = (int)(
                0.01f * (
                    Main.maxTilesX * irradiationSpawnRange.Start.Value
                    + Main.maxTilesX * WorldGen.genRand.Next(0, irradiationSpawnRange.End.Value - irradiationSpawnRange.Start.Value)
                )
            );

            int j = SurfaceHeight(i);
            for (int x = 0; x < irradiationHeight; x++)
            {
                progress.Set((float)x / irradiationHeight / 2f);
                for (int y = 0; y < 8; y++)
                {
                    int radius = (int)(70f * (1f - (float)x / irradiationHeight) + 8f * (MathF.Sin(x * 0.1f) + 1f));
                    ForEachInCircle(
                        i + WorldGen.genRand.NextDirection((int)(radius * 0.25f)..(int)(radius * 0.6f)) + (int)(70f * MathF.Sin(x * 0.1f)),
                        j + x + WorldGen.genRand.NextDirection((int)(radius * 0.1f)..(int)(radius * 0.3f)),
                        (int)(radius * WorldGen.genRand.NextFloat()),
                        (int)(radius * WorldGen.genRand.NextFloat()),
                        FastRemoveTile
                    );
                }
            }

            for (int radius, x = 0; x < irradiationHeight; x += 1 + (int)(radius * 0.1f))
            {
                progress.Set(0.5f + (float)x / irradiationHeight / 2f);
                radius = (int)(300f * (1f - (float)x / irradiationHeight));
                int iOffset = i + WorldGen.genRand.NextDirection(10..130);
                int jOffset = j + x + WorldGen.genRand.NextDirection(20..30);
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
                        if (WorldGen.genRand.NextFloat() < iDistance * 0.5f || WorldGen.genRand.NextFloat() < jDistance * 0.5f)
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
        }

        [Task]
        private void OreTask(GenerationProgress progress)
        {
            /*progress.Message = "Shi";

            ushort protolithType = (ushort)TileType<Protolith>();

            void SpreadOre(ushort oreType, float chance, Range repeatCount, Range sprayRadius, Range blobSize)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = (int)Main.rockLayer; j < Main.maxTilesY; j++)
                    {
                        if (
                            WorldGen.genRand.NextFloat() < chance &&
                            CountConnectedTiles(i, j, tile => tile.HasTile, 35) == 35
                            )
                        {
                            BlobTileRunner(i, j, oreType, repeatCount, sprayRadius, blobSize);
                        }
                    }
                }
            }

            SpreadOre((ushort)TileType<ArtemiteOre>(), 0.00026f, 6..9, 3..5, 7..9);
            progress.Set(0.20f);

            SpreadOre((ushort)TileType<ChandriumOre>(), 0.00026f, 5..7, 6..8, 8..10);
            progress.Set(0.40f);

            SpreadOre((ushort)TileType<DianiteOre>(), 0.00026f, 7..9, 2..4, 6..8);
            progress.Set(0.60f);

            SpreadOre((ushort)TileType<SeleniteOre>(), 0.00026f, 3..5, 6..12, 9..12);
            progress.Set(0.80f);

            SpreadOre(TileID.LunarOre, 0.0002f, 4..6, 7..12, 12..15);
            progress.Set(1f);*/

            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.OrePass");

            int protolithType = TileType<Protolith>();
            GenerateOre(TileType<ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
            GenerateOre(TileType<SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);

            GenerateOre(TileID.LunarOre, 0.00005, WorldGen.genRand.Next(9, 15), WorldGen.genRand.Next(9, 15), protolithType);
        }

        /*[GenPass(nameof(OrePass), InsertMode.After)]
        private void ChestPass(GenerationProgress progress)
        {
            progress.Message = "Chess";

            float chestSpawnChance = 0.003f;
            int protolithType = TileType<Protolith>();
            bool TryPlaceChest(int i, int j)
            {
                TileNeighbourInfo neighbourInfo = new(i, j);
                if (
                    !Main.tile[i, j].HasTile
                    && !neighbourInfo.Solid.Right
                    && !neighbourInfo.Solid.TopRight
                    && !neighbourInfo.Solid.Top
                    && neighbourInfo.Solid.Bottom
                    && neighbourInfo.Solid.BottomRight
                    )
                {
                    WorldGen.PlaceChest(i, j, 21, false, 1);
                    return true;
                }

                return false;
            }

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                progress.Set((float)i / Main.maxTilesX);
                for (int j = (int)Main.rockLayer; j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].TileType == protolithType && WorldGen.genRand.NextFloat() < chestSpawnChance)
                    {
                        TryPlaceChest(i, j - 1);
                    }
                }
            }
        }*/

        // [Task]
        private void OutpostsTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Rooms");

            int retryCount = 0;
            int failCount = 0;
            const int maxRetry = 50;

            int maxSteps = (int)(500 * WorldSize.Current.GetSizeRatio(WorldSize.Small));

            for (int step = 0; step < maxSteps;)
            {
                progress.Set(step / (double)maxSteps);

                int structurePadding = 10;
                int tileX = WorldGen.genRand.Next(structurePadding, Main.maxTilesX - structurePadding);
                int tileY = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - structurePadding);

                Rectangle area = new(tileX, tileY, WorldGen.genRand.Next(15, 26), WorldGen.genRand.Next(14, 20));
                Point origin = new(tileX, tileY);


                bool canPlace = StructureMap.CanPlace(area, structurePadding) &&
                    WorldUtils.Find(new(origin.X, origin.Y + area.Height), Searches.Chain(
                        new Searches.Up(10),
                        new Conditions.IsSolid().AreaAnd(area.Width / 2, 1).Not(),
                        new Conditions.IsTile((ushort)TileType<Protolith>()).AreaOr(area.Width, area.Height)),
                        out _
                    );

                if (!canPlace && retryCount < maxRetry)
                {
                    retryCount++;
                    continue;
                }

                if (retryCount >= maxRetry)
                    failCount++;

                retryCount = 0;

                int wallThickness = 1;
                Point hollowPos = new(tileX + wallThickness, tileY + wallThickness);
                var hollow = new Shapes.Rectangle(area.Width - wallThickness * 2, area.Height - wallThickness * 2);

                bool isRuined = !WorldGen.genRand.NextBool(4);

                if (isRuined)
                {
                    WorldUtils.Gen(origin, new Shapes.Rectangle(area.Width, area.Height),
                    Actions.Chain(
                        new Modifiers.IsSolid(),
                        new Actions.SetTileKeepWall((ushort)TileType<RegolithBrick>()),
                        new Actions.Smooth(),
                        new Actions.SetFrames(frameNeighbors: false)
                        )
                    );

                    WorldUtils.Gen(hollowPos, hollow,
                        Actions.Chain(
                            new Modifiers.IsSolid(),
                            new Actions.PlaceWall((ushort)WallType<RegolithBrickWall>())
                        )
                    );

                    WorldUtils.Gen(hollowPos, hollow, new Actions.ClearTile());
                }
                else
                {
                    WorldUtils.Gen(origin, new Shapes.Rectangle(area.Width, area.Height),
                    Actions.Chain(
                        new Actions.SetTileKeepWall((ushort)TileType<RegolithBrick>()),
                        new Actions.SetFrames(frameNeighbors: false)
                        )
                    );

                    WorldUtils.Gen(hollowPos, hollow,
                    Actions.Chain(
                        new Actions.ClearTile(),
                        new Actions.PlaceWall((ushort)WallType<RegolithBrickWall>())
                        )
                    );
                }

                StructureMap.AddProtectedStructure(area, structurePadding);
                step++;
            }

            if (failCount > 0)
                LogChatMessage($"Failed to place {failCount} Moon Base outposts.");
        }


        [Task]
        private void AmbientTask(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.AmbientPass");
            float smallRockSpawnChance = 0.1f;
            float mediumRockSpawnChance = 0.05f;
            float largeRockSpawnChance = 0.01f;
            ushort regolithType = (ushort)TileType<Regolith>();

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
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockLargeNatural>(), style: WorldGen.genRand.Next(2), mute: true);
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
