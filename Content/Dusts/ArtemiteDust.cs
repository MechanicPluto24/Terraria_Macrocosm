using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class ArtemiteDust : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.rotation += 0.3f * (dust.dustIndex % 2 == 0 ? -1 : 1);
            dust.scale -= 0.02f;

            if (!dust.noGravity)
                dust.velocity.Y += 0.1f;
            else
                dust.velocity *= 0.96f;

            if (dust.scale <= 0f)
                dust.active = false;

            dust.position += dust.velocity;

            return false;
        }
    }
}