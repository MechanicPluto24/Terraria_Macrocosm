using Macrocosm.Content.Projectiles.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class CruithneGreenSlug : ModProjectile, IBullet
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(14);
            AIType = ProjectileID.Bullet;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 270;
            Projectile.light = 0f;
        }

        public override bool PreAI()
        {
            Lighting.AddLight(Projectile.position, new Color(30, 255, 105).ToVector3() * 0.6f);
            return true;
        }
    }
}
