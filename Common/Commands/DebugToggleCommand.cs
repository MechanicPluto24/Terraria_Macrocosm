using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.Navigation.NavigationPanel;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

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
						UIMapTarget.DebugModeActive = !UIMapTarget.DebugModeActive;
						break;

					// TODO: particles, launchpads, etc.

					default:
                        Main.NewText("Unknown argument " + args[0]);
                        break;
            }
        }
    }
}
