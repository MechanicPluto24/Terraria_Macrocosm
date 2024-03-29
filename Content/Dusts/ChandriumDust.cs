using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class ChandriumDust : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
            //dust.scale -= 0.05f; was too short lived 
            dust.scale -= 0.02f;

            float lightMultiplier = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, 0.61f * lightMultiplier, 0.26f * lightMultiplier, 0.85f * lightMultiplier);

            if (dust.scale <= 0f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}