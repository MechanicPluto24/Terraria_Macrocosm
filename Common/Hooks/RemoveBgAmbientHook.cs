using SubworldLibrary;
using Terraria.GameContent.Ambience;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks {
    public class RemoveBgAmbientHook : ILoadable {
        public void Load(Mod mod) {
            On.Terraria.GameContent.Ambience.AmbienceServer.Update += DisableAmbienceOnMoon;
        }

        public void Unload() { }

        private static void DisableAmbienceOnMoon(On.Terraria.GameContent.Ambience.AmbienceServer.orig_Update orig, AmbienceServer self) {
            if (SubworldSystem.AnyActive<Macrocosm>())
                return;

            orig(self);
        }
    }
}