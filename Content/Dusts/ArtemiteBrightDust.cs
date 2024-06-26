using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class ArtemiteBrightDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.dustIndex % 2 == 0 ? -0.1f : 0.1f;

            if (!dust.noGravity)
                dust.velocity.Y += 0.9f;
            else
                dust.velocity *= 0.88f;

            dust.scale -= 0.04f;

            if (dust.scale < 0.2f)
                dust.active = false;

            Lighting.AddLight(dust.position, new Color(51, 185, 131).ToVector3() * 0.6f);

            return false;
        }

        public override bool MidUpdate(Dust dust) => true;

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => Color.White;

    }
}