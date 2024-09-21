using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class CruithneGreenSlug : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(14);
            AIType = -1;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 270;
            Projectile.light = 0f;
        }

        public override bool PreAI()
        {
            Lighting.AddLight(Projectile.position, new Color(30, 255, 105).ToVector3() * 0.6f);

            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawMagicPixelTrail(new Vector2(0, 0), 4f, 0f, new Color(0, 178, 115) * lightColor.GetBrightness(), new Color(255, 255, 255, 74) * lightColor.GetBrightness());
            return true;
        }
    }
}
