using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SubworldLibrary;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Hooks
{
    public class ExplosionHooks : ModSystem
    {
        public override void Load()
        {
            On_Projectile.ExplodeTiles += On_Projectile_ExplodeTiles;
        }

        public override void Unload()
        {
            On_Projectile.ExplodeTiles -= On_Projectile_ExplodeTiles;
        }
        //Though the explosion global tile works well for tiles, it doesn't work for walls :(
        private void On_Projectile_ExplodeTiles(On_Projectile.orig_ExplodeTiles orig,Projectile self, Vector2 compareSpot, int radius, int minI, int maxI, int minJ, int maxJ, bool wallSplode)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                return;
            orig(self,compareSpot, radius, minI, maxI, minJ, maxJ, wallSplode);
        }
    }
}
