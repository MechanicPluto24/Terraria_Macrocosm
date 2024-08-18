using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class WaveGunLaser : RicochetBullet
    {
        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetProjectileDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.position, new Color(255, 255, 0).ToVector3() * 0.6f);
        }
    }
}
