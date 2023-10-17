using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.IO;
using Terraria;
using Terraria.ModLoader;
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
				return;
			#endif

			if (!SubworldSystem.AnyActive())
			{
				WorldUtils.DebugRegen();

				for (int x = 0; x < Main.maxTilesX; x++)
				{
					for (int y = 0; y < Main.maxTilesY; y++)
					{
						WorldGen.TileFrame(x, y, true, true);
						Framing.WallFrame(x, y, true);
					}
				}
			}
			else
			{
				WorldGen.clearWorld();
				SubworldSystem.Current.Tasks.ForEach(t => t.Apply(WorldGenerator.CurrentGenerationProgress = new(), SubworldSystem.Current.Config?.GetPassConfiguration(t.Name)));
				Main.NewText("Subworld regeneration complete");
			}
		}
    }
}
