using Macrocosm.Common.Utils;
using SubworldLibrary;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.Commands
{
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

            Utility.LogChatMessage($"Started world regeneration at {DateTime.Now}");
            var stopwatch = Stopwatch.StartNew();

            if (!SubworldSystem.AnyActive())
            {
                WorldUtils.DebugRegen();
            }
            else
            {

                WorldGen.clearWorld();
                WorldGen._genRand = new UnifiedRandom(Main.ActiveWorldFileData.Seed);
                SubworldSystem.Current.Tasks.ForEach(t => t.Apply(WorldGenerator.CurrentGenerationProgress = new(), SubworldSystem.Current.Config?.GetPassConfiguration(t.Name)));
            }

            stopwatch.Stop();
            Utility.LogChatMessage($"World regeneration complete in {stopwatch.Elapsed}");
            stopwatch.Start();

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
