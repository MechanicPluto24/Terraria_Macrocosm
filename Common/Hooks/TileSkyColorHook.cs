using Macrocosm.Common.Utils;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class TileSkyColorHook : ILoadable
	{
		public void Load(Mod mod)
		{
			On.Terraria.Main.ApplyColorOfTheSkiesToTiles += Main_ApplyColorOfTheSkiesToTiles;
		}
		public void Unload() 
		{
			On.Terraria.Main.ApplyColorOfTheSkiesToTiles -= Main_ApplyColorOfTheSkiesToTiles;
		}

		private void Main_ApplyColorOfTheSkiesToTiles(On.Terraria.Main.orig_ApplyColorOfTheSkiesToTiles orig)
		{
			if (SubworldSystem.IsActive<Moon>())
			{
				Color colorOfTheSkies = Main.ColorOfTheSkies.ToGrayscale();

				Main.tileColor.R = (byte)((colorOfTheSkies.R + colorOfTheSkies.G + colorOfTheSkies.B + colorOfTheSkies.R * 7) / 10);
				Main.tileColor.G = (byte)((colorOfTheSkies.R + colorOfTheSkies.G + colorOfTheSkies.B + colorOfTheSkies.G * 7) / 10);
				Main.tileColor.B = (byte)((colorOfTheSkies.R + colorOfTheSkies.G + colorOfTheSkies.B + colorOfTheSkies.B * 7) / 10);
			}
			else
			{
				orig();
			}
		}


	}
}