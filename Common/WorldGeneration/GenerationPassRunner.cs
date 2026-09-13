using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration;

internal static class GenerationPassRunner
{
    public static void Run(IEnumerable<GenPass> passes, int seed, WorldGenConfiguration configuration)
    {
        var enabledPasses = passes.Where(pass => pass.Enabled).ToArray();
        var previousRandom = Main.rand;
        var previousProgress = WorldGenerator.CurrentGenerationProgress;
        var progress = new GenerationProgress { TotalWeight = enabledPasses.Sum(pass => pass.Weight) };

        try
        {
            WorldGenerator.CurrentGenerationProgress = progress;
            foreach (var pass in enabledPasses)
            {
                if (!pass.Enabled)
                    continue;

                // WorldGen.genRand aliases Main.rand; vanilla WorldGenerator reseeds before each pass.
                Main.rand = new UnifiedRandom(seed);
                progress.Start(pass.Weight);
                pass.Apply(progress, configuration?.GetPassConfiguration(pass.Name));
                progress.End();
            }
        }
        finally
        {
            Main.rand = previousRandom;
            WorldGenerator.CurrentGenerationProgress = previousProgress;
        }
    }
}
