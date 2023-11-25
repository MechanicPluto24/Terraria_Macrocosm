using SubworldLibrary;
using Terraria.GameContent.Skies;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class RemoveBgAmbient : ILoadable
    {
        public void Load(Mod mod)
        {
            On_AmbientSky.Draw += AmbientSky_Draw;
        }

        public void Unload()
        {
            On_AmbientSky.Draw -= AmbientSky_Draw;

        }

        private void AmbientSky_Draw(On_AmbientSky.orig_Draw orig, AmbientSky self, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                return;

            orig(self, spriteBatch, minDepth, maxDepth);
        }
    }
}