using Macrocosm.Common.Sets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class RyuguShell : ModProjectile
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
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 270;
            Projectile.light = 0f;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.alpha = 255;
        }

        public override bool PreAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y += 0.01f;

            if (Projectile.alpha > 0)
                Projectile.alpha -= 15;

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            //Projectile.DrawMagicPixelTrail(new Vector2(0, 0), 4f, 0f, new Color(255, 100, 100) * Projectile.Opacity, new Color(255, 201, 84, 0) * Projectile.Opacity);

            float count = 28 * (1f - Projectile.alpha / 255f);
            for (int n = 0; n < count; n++)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                float opacity = (1f - Projectile.alpha / 255f);
                float progress = n / count;
                Color color = Color.White * (0.7f - progress) * opacity;
                Vector2 trailPosition = Projectile.Center - Projectile.velocity.SafeNormalize(default) * n * 6f;
                Main.EntitySpriteDraw(texture, trailPosition - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1f * (1f - progress), SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
