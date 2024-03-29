using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class MoonSwordDust : ModDust
    {
        public override bool Update(Dust dust)
        {

            dust.position += dust.velocity;
            dust.rotation += dust.dustIndex % 2 == 0 ? 0.5f : -0.5f;

            float clampedScale = dust.scale;
            if (clampedScale > 1f)
                clampedScale = 1f;

            if (!dust.noLight)
                Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), clampedScale * 0.2f, clampedScale * 0.725f, clampedScale * 0.51f);

            if (dust.noGravity)
                dust.velocity *= 0.93f;
            else
                dust.velocity.Y += 0.1f;

            dust.scale -= 0.01f;
            if (dust.scale < 0.2f)
                dust.active = false;

            return false;
        }

        public override bool MidUpdate(Dust dust) => false;

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => Color.White.WithOpacity(0.5f);

        public override bool PreDraw(Dust dust)
        {
            //float count = Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y) * 3f;
            //if (count > 10f)
            //	count = 10f;
            //
            //for (int n = 0; n < count; n++)
            //{
            //	Vector2 trailPosition = dust.position - dust.velocity * n;
            //	float scale = dust.scale * (1f - n / 10f);
            //	Color color = Lighting.GetColor((int)(dust.position.X + 4.0) / 16, (int)(dust.position.Y + 4.0) / 16);
            //	Main.spriteBatch.Draw(dust.GetTexture(), trailPosition - Main.screenPosition, dust.frame, dust.GetAlpha(color), dust.rotation, new Vector2(4f, 4f), scale, SpriteEffects.None, 0f);
            //}

            return true;
        }
    }
}