using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Blocks;
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

            int oceanIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Beaches"));
            if (oceanIndex != -1)
                tasks.Insert(oceanIndex + 1, new PassLegacy("Macrocosm: Silica", GenerateSilicaSand_Ocean));
        }

        private void GenerateOres(GenerationProgress progress, GameConfiguration configuration)
        {
            GenerateAluminum(progress, configuration);
            GenerateLithium(progress, configuration);
            GenerateOilShales(progress, configuration);
            GenerateSilicaSand_Desert(progress, configuration);
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

        private void GenerateOilShales(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY * 0.00035; i++)
            {
                int tileX = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
                int tileY = WorldGen.genRand.Next(GenVars.desertHiveHigh, GenVars.desertHiveLow);

                // Get the progress in the desert hive depth (0 - top, 1 - bottom)
                float depthProgress = (float)(tileY - GenVars.desertHiveHigh) / (GenVars.desertHiveLow - GenVars.desertHiveHigh);

                int topDenominator = 1;
                int bottomDenominator = 40;

                // Scale the chance by depth
                int chance = (int)(topDenominator + depthProgress * bottomDenominator);

                // Chance to place oil shale increases the lower you go
                // TODO: check why this doesn't look right in practice
                if (!Main.rand.NextBool(chance))
                {
                    int type = Main.tile[tileX, tileY].TileType;
                    if (TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type])
                        WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(6, 14), TileType<OilShale>());
                }
            }
        }

        // TODO: decide presence in sand patches, underground desert and surface deserts
        private void GenerateSilicaSand_Desert(GenerationProgress progress, GameConfiguration configuration)
        {

        }

        private void GenerateSilicaSand_Ocean(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY * 0.00015; i++)
            {
                int tileY = WorldGen.genRand.Next((int)GenVars.skyLakes, (int)GenVars.rockLayerHigh);
                int tileX = WorldGen.genRand.Next(0, WorldGen.beachDistance);

                if (WorldGen.genRand.NextBool())
                    tileX = WorldGen.genRand.Next(Main.maxTilesX - WorldGen.beachDistance, Main.maxTilesX);

                int type = Main.tile[tileX, tileY].TileType;

                if (type is TileID.Dirt or TileID.Stone)
                {
                    Utility.BlobTileRunner(tileX, tileY, TileType<SilicaSand>(),
                        repeatCount: 1..2, sprayRadius: 10..40, blobSize: 10..30,
                        perTileCheck: (i, j) => WorldGen.SolidOrSlopedTile(i, j) && (Main.tile[i, j].TileType is TileID.Dirt or TileID.Stone)
                    );
                }

                // In case some mods or seeds add converted oceans (?)
                /*
				if (type is TileID.Ebonsand)
					WorldGen.TileRunner(tileX, tileY, strength, steps, TileType<SilicaEbonsand>());

				if (type is TileID.Crimsand)
					WorldGen.TileRunner(tileX, tileY, strength, steps, TileType<SilicaCrimsand>());
				
				if (type is TileID.Pearlsand)
					WorldGen.TileRunner(tileX, tileY, strength, steps, TileType<SilicaPearlsand>());
				*/
            }
        }
    }
}
