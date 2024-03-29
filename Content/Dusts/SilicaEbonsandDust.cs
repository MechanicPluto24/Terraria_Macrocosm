using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class SilicaEbonsandDust : ModDust
    {
        public override bool Update(Dust dust)
        {
            dust.scale -= 0.01f;
            dust.velocity.X *= 0.96f;

            if (!dust.noGravity)
            {
                dust.velocity.Y += 0.1f;
            }

            return true;
        }
    }
}