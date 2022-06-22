using Terraria;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria.ModLoader;
using Macrocosm.Content.Subworlds.Moon;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Ambience;

namespace Macrocosm.Common.Drawing {
    public sealed class RemoveBackgroundAmbient {
        public static void InitializeDetour() {
            On.Terraria.GameContent.Ambience.AmbienceServer.Update += DisableAmbienceOnMoon;
        }

        private static void DisableAmbienceOnMoon(On.Terraria.GameContent.Ambience.AmbienceServer.orig_Update orig, AmbienceServer self)
        {
            if (SubworldSystem.AnyActive<Macrocosm>()) 
                return;

            orig(self);        
        }
    }
}