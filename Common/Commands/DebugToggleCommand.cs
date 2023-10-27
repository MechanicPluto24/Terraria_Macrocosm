using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.UI;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Commands
{
	public class DebugToggleCommand : ModCommand
    {
        public override string Command => "debug";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            #if !DEBUG
				return;
            #endif

			if (args.Length <= 0)
            {
                Main.NewText("Unknown argument");
				return;
            }
            else switch(args[0].ToLower())
            {
                    case "rockets":
                        RocketManager.DebugModeActive = !RocketManager.DebugModeActive;
						break;

					case "navigation":
						UINavigationTarget.DebugModeActive = !UINavigationTarget.DebugModeActive;
						break;

					// TODO: particles, launchpads, etc.

					default:
                        Main.NewText("Unknown argument " + args[0]);
                        break;
            }
        }
    }
}
