using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Terraria;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class WaveRifleLaser : RicochetBullet
    {
        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override bool CanRicochet() => false;

        public override void SetProjectileDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, new Color(255, 0, 255).ToVector3() * 0.6f);
        }
    }
}
