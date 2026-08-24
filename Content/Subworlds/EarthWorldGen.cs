using Macrocosm.Common.Config;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Liquids;
using Macrocosm.Content.Tiles.Blocks.Sands;
using Macrocosm.Content.Tiles.Ores;
using ModLiquidLib.ModLoader;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds;

class EarthWorldGen : ModSystem
{
    // Aluminum
    private const double AluminumShallowVeinFrequency = 0.00001;
    private const double AluminumDeepVeinFrequency = 0.00008;
    private static readonly Range AluminumShallowStrength = 3..6;
    private static readonly Range AluminumShallowSteps = 3..6;
    private static readonly Range AluminumDeepStrength = 3..7;
    private static readonly Range AluminumDeepSteps = 3..7;

    // Lithium
    private const double LithiumVeinFrequency = 0.0001;
    private static readonly Range LithiumStrength = 4..6;
    private static readonly Range LithiumSteps = 4..6;

    // Coal
    private const double CoalVeinFrequency = 0.00016;
    private static readonly Range CoalStrength = 7..12;
    private static readonly Range CoalSteps = 5..10;

    // Oil Shale
    private const double OilShaleVeinFrequency = 0.00035;
    private static readonly Range OilShaleStrength = 5..7;
    private static readonly Range OilShaleSteps = 8..17;

    // Silica Sand
    private const double UndergroundSilicaVeinFrequency = 0.000008;
    private const double DesertSilicaVeinFrequency = 0.000008;
    private const double OceanSilicaVeinsPerWorldWidth = 0.013;
    private const int OceanSilicaDepthOffset = 100;
    private const int OceanSilicaRemixVeinDivisor = 4;
    private const float OptionalSilicaBlobDensity = 0.65f;
    private const int OptionalSilicaBlobSmoothingPasses = 2;
    private const float OceanSilicaBlobDensity = 0.5f;
    private const int OceanSilicaBlobSmoothingPasses = 4;
    private static readonly Range OptionalSilicaRepeatCount = 1..2;
    private static readonly Range OptionalSilicaSprayRadius = 0..1;
    private static readonly Range OptionalSilicaBlobSize = 4..8;
    private static readonly Range OceanSilicaRepeatCount = 1..10;
    private static readonly Range OceanSilicaSprayRadius = 15..65;
    private static readonly Range OceanSilicaBlobSize = 20..40;

    // Oil
    private const double OilPuddleFrequency = 0.00012;
    private const double DesertOilPuddleFrequency = 0.000005;
    private const int OilPuddleWorldEdgeMargin = 30;
    private const int OilPuddleUnderworldMargin = 30;
    private const int OilPuddleRadius = 5;
    private const int DesertOilPuddleRadius = 5;
    private static readonly Range OilPuddleStrength = 3..6;
    private static readonly Range OilPuddleSteps = 3..15;

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        int shiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
        if (shiniesIndex != -1)
            tasks.Insert(shiniesIndex + 1, new PassLegacy("Macrocosm: Ores", GenerateOres));

        int oceanIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand Patches"));
        if (oceanIndex != -1)
            tasks.Insert(oceanIndex + 1, new PassLegacy("Macrocosm: Silica", GenerateSilicaSand_Ocean));

        int slushIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Slush"));
        if (slushIndex != -1)
            tasks.Insert(slushIndex + 1, new PassLegacy("Macrocosm: Oil", GenerateOil));

        int finalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
        if (finalIndex != -1)
        {
            tasks.Insert(finalIndex + 1, new PassLegacy("Macrocosm: Stabilize Silica", StabilizeSilicaSand));
            tasks.Insert(finalIndex + 2, new PassLegacy("Macrocosm: Desert Oil", GenerateDesertOil));
        }

    }

    /// <summary> Mirrors vanilla late world generation safeguard for unsupported sand. </summary>
    private void StabilizeSilicaSand(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 1; i < Main.maxTilesX - 1; i++)
        {
            progress.Set((double)i / Main.maxTilesX);

            for (int j = 1; j < Main.maxTilesY - 1; j++)
            {
                Tile tile = Main.tile[i, j];
                if (!tile.HasTile || !TryGetHardenedSilicaSupport(tile.TileType, out ushort supportType) || WorldGen.SolidTile(i, j + 1))
                    continue;

                Tile tileBelow = Main.tile[i, j + 1];
                if (tileBelow.HasTile && Main.tileSolid[tileBelow.TileType] && !Main.tileSolidTop[tileBelow.TileType] && (tileBelow.Slope != SlopeType.Solid || tileBelow.IsHalfBlock))
                {
                    tileBelow.Slope = SlopeType.Solid;
                    tileBelow.IsHalfBlock = false;
                }
                else
                {
                    tile.TileType = supportType;
                }
            }
        }
    }

    private static bool TryGetHardenedSilicaSupport(ushort silicaType, out ushort supportType)
    {
        if (silicaType == TileType<SilicaSand>())
            supportType = TileID.HardenedSand;
        else if (silicaType == TileType<SilicaEbonsand>())
            supportType = TileID.CorruptHardenedSand;
        else if (silicaType == TileType<SilicaCrimsand>())
            supportType = TileID.CrimsonHardenedSand;
        else if (silicaType == TileType<SilicaPearlsand>())
            supportType = TileID.HallowHardenedSand;
        else
        {
            supportType = 0;
            return false;
        }

        return true;
    }

    private void GenerateOil(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * OilPuddleFrequency); i++)
        {
            int X = WorldGen.genRand.Next(OilPuddleWorldEdgeMargin, Main.maxTilesX - OilPuddleWorldEdgeMargin);
            int Y = WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer - OilPuddleUnderworldMargin);

            if (Main.tile[X, Y].HasTile)
            {
                WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(OilPuddleStrength), WorldGen.genRand.Next(OilPuddleSteps), -1);

                Utility.ForEachInCircle(
                    X,
                    Y,
                    OilPuddleRadius,
                    (i1, j1) =>
                    {
                        if (!WorldGen.InWorld(i1, j1))
                            return;

                        if (!Main.tile[i1, j1].HasTile)
                        {
                            WorldGen.PlaceLiquid(i1, j1, (byte)LiquidLoader.LiquidType<Oil>(), byte.MaxValue);
                        }
                    }
                );
            }
        }
    }

    private void GenerateDesertOil(GenerationProgress progress, GameConfiguration configuration)
    {

        for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * DesertOilPuddleFrequency); i++)
        {
            int X = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.desertHiveLow);

            Utility.ForEachInCircle(
                X,
                Y,
                DesertOilPuddleRadius,
                (i1, j1) =>
                {
                    if (!WorldGen.InWorld(i1, j1))
                        return;

                    if (!Main.tile[i1, j1].HasTile)
                    {
                        WorldGen.PlaceLiquid(i1, j1, (byte)LiquidLoader.LiquidType<Oil>(), byte.MaxValue);
                    }
                }
            );
        }
    }

    private void GenerateOres(GenerationProgress progress, GameConfiguration configuration)
    {
        GenerateAluminum(progress, configuration);
        GenerateLithium(progress, configuration);
        GenerateCoal(progress, configuration);
        GenerateOilShales(progress, configuration);

        // TODO: these need some attention
        //GenerateSilicaSand_Underground(progress, configuration);
        //GenerateSilicaSand_Desert(progress, configuration);
    }

    private void GenerateAluminum(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * AluminumShallowVeinFrequency); i++)
            WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(AluminumShallowStrength), WorldGen.genRand.Next(AluminumShallowSteps), TileType<AluminumOre>());

        for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * AluminumDeepVeinFrequency); i++)
            WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(AluminumDeepStrength), WorldGen.genRand.Next(AluminumDeepSteps), TileType<AluminumOre>());
    }

    private void GenerateLithium(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * LithiumVeinFrequency); i++)
            WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(LithiumStrength), WorldGen.genRand.Next(LithiumSteps), TileType<LithiumOre>());
    }

    private void GenerateCoal(GenerationProgress progress, GameConfiguration configuration)
    {
        for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * CoalVeinFrequency); i++)
            WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY), WorldGen.genRand.Next(CoalStrength), WorldGen.genRand.Next(CoalSteps), TileType<Coal>());
    }

    private void GenerateOilShales(GenerationProgress progress, GameConfiguration configuration)
    {
        if (ServerConfig.Instance.DisableOilShaleGeneration)
            return;

        if (!IsUndergroundDesertValid())
            return;

        for (int i = 0; i < Main.maxTilesX * Main.maxTilesY * OilShaleVeinFrequency; i++)
        {
            int tileX = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
            int tileY = WorldGen.genRand.Next(GenVars.desertHiveHigh, GenVars.desertHiveLow);

            float depthProgress = (float)(tileY - GenVars.desertHiveHigh) / (GenVars.desertHiveLow - GenVars.desertHiveHigh);

            if (Main.rand.NextFloat() < depthProgress)
            {
                int type = Main.tile[tileX, tileY].TileType;
                if (TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type])
                    WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(OilShaleStrength), WorldGen.genRand.Next(OilShaleSteps), TileType<OilShale>());
            }
        }
    }

    /// <summary> Underground Silica generation. </summary>
    private void GenerateSilicaSand_Underground(GenerationProgress progress, GameConfiguration configuration)
    {
        if (ServerConfig.Instance.DisableSilicaSandGeneration)
            return;

        int minY = System.Math.Clamp((int)System.Math.Min(GenVars.rockLayerHigh, GenVars.rockLayerLow), 1, Main.maxTilesY - 2);
        int maxY = System.Math.Clamp((int)System.Math.Max(GenVars.rockLayerHigh, GenVars.rockLayerLow), minY + 1, Main.maxTilesY - 1);
        int veinCount = (int)(Main.maxTilesX * Main.maxTilesY * UndergroundSilicaVeinFrequency);

        for (int vein = 0; vein < veinCount; vein++)
        {
            progress.Set((double)vein / veinCount);

            int x = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
            int y = WorldGen.genRand.Next(minY, maxY);
            if (!CanReplaceWithUndergroundSilica(x, y))
                continue;

            Utility.BlobTileRunner(
                x, y, TileType<SilicaSand>(),
                repeatCount: OptionalSilicaRepeatCount, sprayRadius: OptionalSilicaSprayRadius, blobSize: OptionalSilicaBlobSize,
                density: OptionalSilicaBlobDensity, smoothing: OptionalSilicaBlobSmoothingPasses,
                perTileCheck: CanReplaceWithUndergroundSilica,
                smoothingPlacementCheck: CanReplaceWithUndergroundSilica
            );
        }
    }

    /// <summary>  Underground Desert Silica generation. </summary>
    private void GenerateSilicaSand_Desert(GenerationProgress progress, GameConfiguration configuration)
    {
        if (ServerConfig.Instance.DisableSilicaSandGeneration || !IsUndergroundDesertValid())
            return;

        int veinCount = (int)(Main.maxTilesX * Main.maxTilesY * DesertSilicaVeinFrequency);

        for (int vein = 0; vein < veinCount; vein++)
        {
            progress.Set((double)vein / veinCount);

            int x = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
            int y = WorldGen.genRand.Next(GenVars.desertHiveHigh, GenVars.desertHiveLow);
            float depthProgress = (float)(y - GenVars.desertHiveHigh) / (GenVars.desertHiveLow - GenVars.desertHiveHigh);

            if (WorldGen.genRand.NextFloat() >= depthProgress || !CanReplaceWithDesertSilica(x, y))
                continue;

            Utility.BlobTileRunner(
                x, y, TileType<SilicaSand>(),
                repeatCount: OptionalSilicaRepeatCount, sprayRadius: OptionalSilicaSprayRadius, blobSize: OptionalSilicaBlobSize,
                density: OptionalSilicaBlobDensity, smoothing: OptionalSilicaBlobSmoothingPasses,
                perTileCheck: CanReplaceWithDesertSilica,
                smoothingPlacementCheck: CanReplaceWithDesertSilica
            );
        }
    }

    private static bool IsUndergroundDesertValid()
    {
        return GenVars.desertHiveLeft >= 0
            && GenVars.desertHiveRight <= Main.maxTilesX
            && GenVars.desertHiveLeft < GenVars.desertHiveRight
            && GenVars.desertHiveHigh >= 0
            && GenVars.desertHiveLow <= Main.maxTilesY
            && GenVars.desertHiveHigh < GenVars.desertHiveLow;
    }


    private void GenerateSilicaSand_Ocean(GenerationProgress progress, GameConfiguration configuration)
    {
        if (ServerConfig.Instance.DisableSilicaSandGeneration)
            return;

        int maxVeins = (int)(Main.maxTilesX * OceanSilicaVeinsPerWorldWidth);
        if (WorldGen.remixWorldGen)
            maxVeins /= OceanSilicaRemixVeinDivisor;

        for (int i = 0; i < maxVeins; i++)
        {
            int x = WorldGen.genRand.Next(0, WorldGen.beachDistance);
            int y = WorldGen.genRand.Next((int)Main.worldSurface + OceanSilicaDepthOffset, (int)GenVars.rockLayerHigh + OceanSilicaDepthOffset);

            if (WorldGen.genRand.NextBool())
                x = WorldGen.genRand.Next(Main.maxTilesX - WorldGen.beachDistance, Main.maxTilesX);

            Utility.BlobTileRunner(
                x, y, TileType<SilicaSand>(),
                repeatCount: OceanSilicaRepeatCount, sprayRadius: OceanSilicaSprayRadius, blobSize: OceanSilicaBlobSize,
                density: OceanSilicaBlobDensity, smoothing: OceanSilicaBlobSmoothingPasses,
                perTileCheck: CanReplaceWithUndergroundSilica,
                smoothingPlacementCheck: CanReplaceWithUndergroundSilica
            );
        }

            private static bool CanReplaceWithUndergroundSilica(int i, int j)
    {
        if (!WorldGen.InWorld(i, j, 1) || !WorldGen.SolidTile(i, j))
            return false;

        ushort type = Main.tile[i, j].TileType;
        return TileID.Sets.Dirt[type] || TileID.Sets.Stone[type];
    }

    private static bool CanReplaceWithDesertSilica(int i, int j)
    {
        if (!WorldGen.InWorld(i, j, 1) || !WorldGen.SolidTile(i, j))
            return false;

        ushort type = Main.tile[i, j].TileType;
        return TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type];
    }
}
}
