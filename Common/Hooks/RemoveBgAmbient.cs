using SubworldLibrary;
using Terraria.GameContent.Ambience;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
	public class RemoveBgAmbient : ILoadable
	{
		public void Load(Mod mod)
		{
			Terraria.GameContent.Ambience.On_AmbienceServer.Update += AmbienceServer_Update;
		}

		public void Unload()
		{
			Terraria.GameContent.Ambience.On_AmbienceServer.Update -= AmbienceServer_Update;
		}

		private void AmbienceServer_Update(Terraria.GameContent.Ambience.On_AmbienceServer.orig_Update orig, AmbienceServer self)
		{
			if (SubworldSystem.AnyActive<Macrocosm>())
				return;

			orig(self);
		}
	}
}