using Macrocosm.Common.DataStructures;
using Macrocosm.Content.Tiles.Ambient;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Macrocosm.Content.Tiles.Blocks;
using static Macrocosm.Common.Utils.Utility;
using Macrocosm.Content.Tiles.Walls;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Ores;

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
            progress.Message = "Terrain";

            Range hallownest = 35..55;
            ushort protolithType = (ushort)TileType<Protolith>();

            Main.worldSurface = GroundY + SurfaceHeightFrequency * 2;
            Main.rockLayer = GroundY + RegolithLayerHeight;

            PerlinNoise2D noise = new(WorldGen.genRand.Next());
            StartYOffset = Main.rand.NextFloat() * 2.3f;

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int startJ = SurfaceHeight(i);
                for (int j = startJ; j < Main.maxTilesY; j++)
                {
                    progress.Set((float)(j + i * Main.maxTilesY) / (Main.maxTilesX * Main.maxTilesY));
                    if (
                        WorldGen.genRand.NextFloat() < 1f - 0.01f * (
                            hallownest.Start.Value + (hallownest.End.Value - hallownest.Start.Value) * MathF.Sin((float)(j - startJ) / (Main.maxTilesY - startJ) * MathHelper.Pi))
                        || noise.GetValue(i * 0.05f, j * 0.05f) > 0f
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
        }

        [Task]
        private void WallTask()
        {
            int regolithWall = WallType<RegolithWall>();
            int protolithWall = WallType<ProtolithWall>();

            PerlinNoise2D wallNoise = new(WorldGen.genRand.Next());

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int wallPlaceStart = SurfaceHeight(i) + 15;
                for (int j = wallPlaceStart; j < Main.maxTilesY; j++)
                {
                    bool placeRegolith = j < wallPlaceStart + RegolithLayerHeight;
                    if (!placeRegolith && wallNoise.GetValue(i * 0.06f, j * 0.06f) > 0.05f)
                    {
                        continue;
                    }

                    FastPlaceWall(i, j, placeRegolith ? regolithWall : protolithWall);
                }
            }
        }

        [Task]
        private void CraterTask(GenerationProgress progress)
        {
            progress.Message = "Metoeoroer";

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
                            Vector2 randomDirection = Main.rand.NextVector2Unit();
                            float offset = 17f * Main.rand.NextFloat(0.7f, 1f);
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
            progress.Message = "Smoothie";

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
                );
            }

            ForEachInRectangle(
                0,
                0,
                Main.maxTilesX,
                Main.maxTilesY,
                (i, j) =>
                {
                    if (WorldGen.genRand.NextBool(3))
                    {
                        SlopeTile(i, j);
                    }
                }
            );
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

        /*[GenPass(nameof(WallPass), InsertMode.After)]
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
        }*/

        [Task]
        private void OreTask(GenerationProgress progress)
        {
            progress.Message = "Shi";

            ushort protolithType = (ushort)TileType<Protolith>();

            void SpreadOre(ushort oreType, float chance, Range repeatCount, Range sprayRadius, Range blobSize)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = (int)Main.rockLayer; j < Main.maxTilesY; j++)
                    {
                        if (
                            Main.tile[i, j].HasTile && Main.tile[i, j].TileType == protolithType 
                            && WorldGen.genRand.NextFloat() < chance &&
                            new TileNeighbourInfo(i, j).TypedSolid(protolithType).Count > 2
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
            progress.Set(1f);
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
                    if (Main.tile[i, j].TileType == protolithType && Main.rand.NextFloat() < chestSpawnChance)
                    {
                        TryPlaceChest(i, j - 1);
                    }
                }
            }
        }*/

        [Task]
        private void AmbientTask(GenerationProgress progress)
        {
            progress.Message = "Sprinkles";
            float smallRockSpawnChance = 0.1f;
            float mediumRockSpawnChance = 0.05f;
            ushort regolithType = (ushort)TileType<Regolith>();

            for (int i = 0; i < Main.maxTilesX - 1; i++)
            {
                progress.Set((float)i / Main.maxTilesX);
                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    TileNeighbourInfo neighbourInfo = new(i, j);
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == regolithType)
                    {
                        if (WorldGen.genRand.NextFloat() < smallRockSpawnChance)
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockSmallNatural>(), style: WorldGen.genRand.Next(10), mute: true);
                        }
                        else if (
                            neighbourInfo.Solid.Right
                            && !neighbourInfo.Solid.Top
                            && !neighbourInfo.Solid.TopRight
                            && WorldGen.genRand.NextFloat() < mediumRockSpawnChance
                            )
                        {
                            WorldGen.PlaceTile(i, j - 1, TileType<RegolithRockMediumNatural>(), style: WorldGen.genRand.Next(6), mute: true);
                        }
                    }
                }
            }
        }

        [Task]
        private void SpawnTask(GenerationProgress progress)
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
