using SubworldLibrary;
using Terraria;
using Terraria.GameContent.Ambience;
using Terraria.GameContent.Skies;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks;

public class AmbientSkyHooks : ILoadable
{
    public void Load(Mod mod)
    {
        On_AmbientSky.Draw += AmbientSky_Draw;
        On_AmbientSky.Spawn += On_AmbientSky_Spawn;
    }

    public void Unload()
    {
        On_AmbientSky.Draw -= AmbientSky_Draw;
        On_AmbientSky.Spawn -= On_AmbientSky_Spawn;
    }

    private void AmbientSky_Draw(On_AmbientSky.orig_Draw orig, AmbientSky self, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        //if (SubworldSystem.AnyActive<Macrocosm>())
        //    return;

        orig(self, spriteBatch, minDepth, maxDepth);
    }

    private void On_AmbientSky_Spawn(On_AmbientSky.orig_Spawn orig, AmbientSky self, Player player, SkyEntityType type, int seed)
    {
        if (SubworldSystem.AnyActive<Macrocosm>() && type != SkyEntityType.Meteor)
            return;

        orig(self, player, type, seed);
    }
}