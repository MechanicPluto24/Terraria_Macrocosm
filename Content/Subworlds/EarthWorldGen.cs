using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Blocks.Sands;
using Macrocosm.Content.Tiles.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Subworlds
{
    class EarthWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int shiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (shiniesIndex != -1)
                tasks.Insert(shiniesIndex + 1, new PassLegacy("Macrocosm: Ores", GenerateOres));

            int oceanIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand Patches"));
            if (oceanIndex != -1)
                tasks.Insert(oceanIndex + 1, new PassLegacy("Macrocosm: Silica", GenerateSilicaSand_Ocean));
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
            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.00001); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), TileType<AluminumOre>());

            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.00008); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), TileType<AluminumOre>());
        }

        private void GenerateLithium(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.0001); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(4, 6), TileType<LithiumOre>());
        }

        private void GenerateCoal(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.00008); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), TileType<Coal>());
        }

        private void GenerateOilShales(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY * 0.0005; i++)
            {
                int tileX = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
                int tileY = WorldGen.genRand.Next(GenVars.desertHiveHigh, GenVars.desertHiveLow);

                float depthProgress = (float)(tileY - GenVars.desertHiveHigh) / (GenVars.desertHiveLow - GenVars.desertHiveHigh);

                if (Main.rand.NextFloat() < depthProgress)
                {
                    int type = Main.tile[tileX, tileY].TileType;
                    if (TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type])
                        WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(6, 14), TileType<OilShale>());
                }
            }
        }

        // TODO: Avoid snow, ice, mud and the sort...
        private void GenerateSilicaSand_Underground(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.000008); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(7, 9), WorldGen.genRand.Next(5, 7), TileType<SilicaSand>());
        }

        // TODO: This causes weird artifacts due to the gravity of the silica sand
        private void GenerateSilicaSand_Desert(GenerationProgress progress, GameConfiguration configuration)
        {
            // Desert
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY * 0.0005; i++)
            {
                int tileX = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
                int tileY = WorldGen.genRand.Next(GenVars.desertHiveHigh, GenVars.desertHiveLow);

                float depthProgress = (float)(tileY - GenVars.desertHiveHigh) / (GenVars.desertHiveLow - GenVars.desertHiveHigh);

                if (Main.rand.NextFloat() < depthProgress)
                {
                    int type = Main.tile[tileX, tileY].TileType;
                    if (TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type])
                        Utility.BlobTileRunner(
                            tileX, tileY, TileType<SilicaSand>(),
                            repeatCount: 1..2, sprayRadius: 10..40, blobSize: 10..30,
                            perTileCheck: (i, j) => WorldGen.SolidOrSlopedTile(i, j) && (
                                TileID.Sets.Conversion.Sand[Main.tile[i, j].TileType] ||
                                TileID.Sets.Conversion.HardenedSand[Main.tile[i, j].TileType] ||
                                TileID.Sets.Conversion.Sandstone[Main.tile[i, j].TileType])
                        );
                }
            }
        }

        private void GenerateSilicaSand_Ocean(GenerationProgress progress, GameConfiguration configuration)
        {
            int maxPatches = (int)(Main.maxTilesX * 0.013);
            if (WorldGen.remixWorldGen)
                maxPatches /= 4;

            for (int i = 0; i < maxPatches; i++)
            {
                int x = WorldGen.genRand.Next(0, WorldGen.beachDistance);
                int y = WorldGen.genRand.Next((int)Main.worldSurface, (int)GenVars.rockLayerHigh);

                if (WorldGen.genRand.NextBool())
                    x = WorldGen.genRand.Next(Main.maxTilesX - WorldGen.beachDistance, Main.maxTilesX);

                int strength = WorldGen.genRand.Next(15, 70);
                int steps = WorldGen.genRand.Next(20, 130);
                WorldGen.TileRunner(x, y, strength, steps, TileType<SilicaSand>());
            }
        }
    }
}
