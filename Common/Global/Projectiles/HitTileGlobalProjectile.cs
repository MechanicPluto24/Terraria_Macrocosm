using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.Projectiles;

public class HitTileGlobalProjectile : GlobalProjectile
{
    public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
    {
        if (ProjectileSets.HitsTiles[projectile.type])
            Collision.HitTiles(projectile.position, oldVelocity, projectile.width, projectile.height);

        return true;
    }
}