using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Commands
{
    public class ClearSubworldCommand : ModCommand
    {
        public override string Command => "clrd";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            #if !DEBUG
                Main.NewText("You must be in debug mode to use this!");
                return;
            #endif

            if (args.Length == 0)
            {
                Main.NewText("Please type in the subworld name to clear data.", Color.Red);
                return;
            }

            string subworldDirectory = Main.worldPathName[..^4];
            if (Directory.Exists(subworldDirectory))
            {
                void DeleteWorldWithExtensionIfExists(string extension)
                {
                    string fileName = $"Macrocosm_{args[0]}.{extension}";
                    string path = Path.Combine(subworldDirectory, fileName);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        Main.NewText($"Successfully deleted {fileName}");
                    }
                }

                DeleteWorldWithExtensionIfExists("wld");
                DeleteWorldWithExtensionIfExists("twld");
                DeleteWorldWithExtensionIfExists("wld.bak");
                DeleteWorldWithExtensionIfExists("wld.bak2");
                DeleteWorldWithExtensionIfExists("twld.bak");
            }
        }
    }
}
