using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class InvarBits : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.dustIndex % 2 == 0 ? 0.5f : -0.5f;

            dust.velocity *= 0.84f;

            dust.scale *= 0.8f;

            if (dust.scale < 0.2f)
                dust.active = false;

            Lighting.AddLight(dust.position, new Color(177, 230, 204).ToVector3() * 0.4f);

            return false;
        }

        public override bool MidUpdate(Dust dust) => true;

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => Color.White.WithAlpha(200);

        public override bool PreDraw(Dust dust)
        {
            return true;
        }
    }
}