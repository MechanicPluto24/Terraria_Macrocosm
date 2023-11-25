using Macrocosm.Common.Bases;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class ArchersLineProjectile : RicochetBullet
    {
        public override int RicochetCount => 10;

        public override float RicochetSpeed => 20f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetProjectileDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.position, new Color(255, 0, 0).ToVector3() * 0.6f);
        }

        public override void OnHitNPC(bool didRicochet, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage -= 10;
        }

        public override void OnHitNPCEffect(bool didRicochet, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (didRicochet)
                SoundEngine.PlaySound(SFX.Ricochet with { Volume = 0.3f }, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(Vector2.Zero, 2f, 0.5f, new Color(254, 121, 2) * lightColor.GetLuminanceNTSC(), new Color(184, 58, 24, 0) * lightColor.GetLuminanceNTSC());
            return true;
        }
    }
}
