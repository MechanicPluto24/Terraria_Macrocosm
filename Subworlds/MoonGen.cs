using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using Macrocosm.Tiles;

namespace Macrocosm.Subworlds
{
    public class MoonGen : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (ShiniesIndex != -1)
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Artemite Gen", LunarOreGen));
        }
        private void LunarOreGen(GenerationProgress progress)
        {
            progress.Message = "Generating lunar ores";
            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * .002); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayerHigh, Main.maxTilesY);
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(4, 7), ModContent.TileType<ArtemiteOre>());
            }
        }
    }
}
