using Macrocosm.Common.Debugging;
using Macrocosm.Common.Debugging.Stats;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Content.Rockets;
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
            else switch (args[0].ToLower())
                {
                    case "rocket" or "rockets":
                        RocketManager.DebugModeActive = !RocketManager.DebugModeActive;
                        Main.NewText($"Rocket debug: {RocketManager.DebugModeActive}");
                        break;

                    case "ui" or "rocketui":
                        UISystem.DebugModeActive = !UISystem.DebugModeActive;
                        Main.NewText($"UI debug: {UISystem.DebugModeActive}");
                        break;

                    case "packets" or "packet" or "packethandler":
                        PacketHandler.DebugModeActive = !PacketHandler.DebugModeActive;
                        Main.NewText($"Packet debug: {PacketHandler.DebugModeActive}");
                        break;

                    case "tilecoords":
                        DebugDrawing.DrawCursorTileCoords = !DebugDrawing.DrawCursorTileCoords;
                        Main.NewText($"Tile coords debug: {DebugDrawing.DrawCursorTileCoords}");
                        break;

                    case "worldcoords":
                        DebugDrawing.DrawCursorWorldCoords = !DebugDrawing.DrawCursorWorldCoords;
                        Main.NewText($"World coords debug: {DebugDrawing.DrawCursorWorldCoords}");
                        break;

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
