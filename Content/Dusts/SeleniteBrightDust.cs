using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class SeleniteBrightDust : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.dustIndex % 2 == 0 ? 0.5f : -0.5f;

            if (!dust.noGravity)
                dust.velocity.Y += 0.9f;
            else
                dust.velocity *= 0.92f;

            dust.scale -= 0.05f;

            if (dust.scale < 0.2f)
                dust.active = false;

            Lighting.AddLight(dust.position, new Color(177, 230, 204).ToVector3() * 0.4f);

            return false;
        }

        public override bool MidUpdate(Dust dust) => true;

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => Color.White.WithAlpha(127);

        public override bool PreDraw(Dust dust)
        {
            //float count = Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y) * 3f;
            //
            //if (count > 6f)
            //	count = 6f;
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