using Macrocosm.Common.Drawing.Trails;
using Macrocosm.Common.Netcode;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Global.Projectiles
{
    public class ExplosiveGlobalProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.ModProjectile is IExplosive)
            {
                projectile.penetrate = -1;
                projectile.tileCollide = true;
            }
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (projectile.ModProjectile is IExplosive explosive)
            {
                // Explosion logic (hitbox, timeleft, penetrate, etc.)
                explosive.OnHit(projectile);

                // Hit tiles visually, in a smaller area than the blast radius
                Collision.HitTiles(projectile.position, oldVelocity, (int)(projectile.width * 0.6f), (int)(projectile.height * 0.6f));

                return false;
            }

            return true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.ModProjectile is IExplosive explosive)
                explosive.OnHit(projectile);
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (projectile.ModProjectile is IExplosive explosive)
                explosive.OnHit(projectile);
        }
    }
}