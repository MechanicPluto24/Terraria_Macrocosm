using SubworldLibrary;
using Terraria.GameContent.Ambience;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
	public class RemoveBgAmbient : ILoadable
	{
		public void Load(Mod mod)
		{
			On.Terraria.GameContent.Ambience.AmbienceServer.Update += AmbienceServer_Update;
		}

		public void Unload()
		{
			On.Terraria.GameContent.Ambience.AmbienceServer.Update -= AmbienceServer_Update;
		}

		private void AmbienceServer_Update(On.Terraria.GameContent.Ambience.AmbienceServer.orig_Update orig, AmbienceServer self)
		{
			if (SubworldSystem.AnyActive<Macrocosm>())
				return;

			orig(self);
		}
	}
}