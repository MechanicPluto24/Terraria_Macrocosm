using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using SubworldLibrary;
using System;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.Commands;

public class WorldRegenCommand : ModCommand
{
    public override string Command => "regen";

    public override CommandType Type => CommandType.Chat;

    public override void Action(CommandCaller caller, string input, string[] args)
    {
#if !DEBUG
				Main.NewText("You must be in debug mode to use this!");
				return;
#endif

        bool doRegen = args.Length == 0 || args.Contains("gen");
        bool doReframe = args.Contains("frame") || args.Contains("reframe");
        bool doSmooth = args.Contains("smooth");

        if (doRegen)
        {
            Utility.LogChatMessage($"Started world regeneration at {DateTime.Now}");
            var stopwatch = Stopwatch.StartNew();

            bool previousGameMenu = Main.gameMenu;
            var previousRandom = Main.rand;
            var previousProgress = WorldGenerator.CurrentGenerationProgress;
            try
            {
                Main.gameMenu = true;
                if (!SubworldSystem.AnyActive())
                {
                    WorldUtils.DebugRegen();
                }
                else
                {
                    int seed = Main.ActiveWorldFileData.Seed;
                    Main.rand = new UnifiedRandom(seed);
                    WorldGen.clearWorld();
                    GenerationPassRunner.Run(SubworldSystem.Current.Tasks, seed, SubworldSystem.Current.Config);
                }
            }
            finally
            {
                Main.rand = previousRandom;
                WorldGenerator.CurrentGenerationProgress = previousProgress;
                Main.gameMenu = previousGameMenu;
            }

            stopwatch.Stop();
            Utility.LogChatMessage($"World regeneration complete in {stopwatch.Elapsed}");

            doReframe = true;
        }

        if (doSmooth)
        {
            Utility.SmoothWorld(new GenerationProgress());
        }

        if (doReframe)
        {
            var stopwatch = Stopwatch.StartNew();
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    WorldGen.TileFrame(x, y, true, true);
                    Framing.WallFrame(x, y, true);
                }
            }

            stopwatch.Stop();
            Utility.LogChatMessage($"Reframe complete in {stopwatch.Elapsed}");
        }
    }
}
