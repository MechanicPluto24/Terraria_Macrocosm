using SubworldLibrary;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
	public class StopEventsHook : ILoadable
	{
		public void Load(Mod mod)
		{
			On.Terraria.Main.StartSlimeRain += DisableSlimeRain;
		}

		public void Unload() { }

		private static void DisableSlimeRain(On.Terraria.Main.orig_StartSlimeRain orig, bool announce)
		{
			if (SubworldSystem.AnyActive<Macrocosm>())
				return;

			orig(announce);
		}
	}
}