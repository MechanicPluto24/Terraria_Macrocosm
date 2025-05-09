using Macrocosm.Common.Debugging.Stats;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Commands
{
    public class DebugToggleCommand : ModCommand
    {
        public override string Command => "debug";

        public override CommandType Type => CommandType.Chat | CommandType.Console;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
#if !DEBUG
                Main.NewText("You must be in debug mode to use this!");
				return;
#endif

            if (args.Length <= 0)
            {
                Main.NewText("Unknown argument");
                return;
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "stats":
                        ContentStats.Analyze(Macrocosm.Instance);
                        Main.NewText("Exported content stats");
                        break;

                    case "gc":
                        GC.Collect();
                        break;

                    default:
                        Main.NewText("Unknown argument " + args[0]);
                        break;
                }
            }
        }
    }
}
