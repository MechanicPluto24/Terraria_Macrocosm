using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Dusts
{
    public class RegolithDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            dust.scale *= 1f;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.01f;
            if (dust.scale < 0f)
            {
                dust.active = false;
            }
            return false;
        }

        public override bool MidUpdate(Dust dust)
        {
            dust.rotation += dust.velocity.X / 3f;
            return true;
        }


        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, lightColor.B, 25);
        }
    }
}