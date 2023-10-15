using Macrocosm.Content.Tiles.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Subworlds
{
    class EarthWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Macrocosm Earth Worldgen", GenerateOres));
            }
        }

        private void GenerateOres(GenerationProgress progress, GameConfiguration configuration)
        {
            int aluminumType = ModContent.TileType<AluminumOre>();
            int lithiumType = ModContent.TileType<LithiumOre>();
            int oilShaleType = ModContent.TileType<OilShale>();

            #region Aluminum Generation 
            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 8E-05); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), aluminumType);

            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.0002); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), aluminumType);
            #endregion

            #region Lithium Generation
            for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.0001); i++)
                WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(4, 6), lithiumType);
            #endregion

            #region Oil Shale Generation
            for (int i = 0; i < Main.maxTilesX * Main.maxTilesY * 0.00012; i++)
            {
                int tileX = WorldGen.genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
                int tileY = WorldGen.genRand.Next(GenVars.desertHiveHigh, GenVars.desertHiveLow);

                int type = Main.tile[tileX, tileY].TileType;

                bool chance = Main.rand.NextBool(15);

                if (!chance && tileY > GenVars.desertHiveHigh + (GenVars.desertHiveLow - GenVars.desertHiveHigh) * 0.33f)
                    chance = Main.rand.NextBool(5);

                if (!chance && tileY > GenVars.desertHiveHigh + (GenVars.desertHiveLow - GenVars.desertHiveHigh) * 0.66f)
                    chance = Main.rand.NextBool();

                if (chance && TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Sandstone[type])
                    WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(6, 14), oilShaleType);
            }
            #endregion
        }
    }
}
