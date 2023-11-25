using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class PortalDarkOrangeDust : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor) => Color.White;
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = false;
            dust.color = Color.White;
            dust.alpha = 255;
            dust.frame = new Rectangle(30, Main.rand.Next(0, 2) * 10, 10, 10);
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData != null && dust.customData is Projectile projectile)
            {
                if (projectile.active)
                    dust.position += projectile.position - projectile.oldPosition;
            }
            else if (dust.customData != null && dust.customData is Vector2)
            {
                Vector2 vector = (Vector2)dust.customData - dust.position;
                if (vector != Vector2.Zero)
                    vector.Normalize();

                dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;
            }

            dust.position += dust.velocity;
            dust.rotation += 0.35f * (dust.dustIndex % 2 == 0 ? -1 : 1);
            dust.scale -= 0.1f;

            if (dust.scale <= 0.01f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}