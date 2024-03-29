﻿using Macrocosm.Common.Debugging;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Systems;
using Macrocosm.Content.Rockets;
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
			else switch (args[0].ToLower())
				{
					case "rockets":
						RocketManager.DebugModeActive = !RocketManager.DebugModeActive;
						break;

					case "ui" or "rocketui":
						UISystem.DebugModeActive = !UISystem.DebugModeActive;
						break;

					case "packets" or "packet" or "packethandler":
						PacketHandler.DebugModeActive = !PacketHandler.DebugModeActive;
						break;

					case "tilecoords":
						DebugDrawing.DrawCursorTileCoords = !DebugDrawing.DrawCursorTileCoords;
						break;

                    case "worldcoords":
                        DebugDrawing.DrawCursorWorldCoords = !DebugDrawing.DrawCursorWorldCoords;
                        break;

                    default:
						Main.NewText("Unknown argument " + args[0]);
						break;
				}
		}
	}
}
