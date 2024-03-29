using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class LuminiteDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = true;
            dust.scale = Main.rand.NextFloat(1, 1.35f);
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale -= 0.02f;
            dust.rotation += (dust.velocity.Y - dust.velocity.X) / 5;

            if (!dust.noGravity)
                dust.velocity.Y += 0.01f;
            else
                dust.velocity *= 0.88f;

            if (dust.scale < 0f)
                dust.active = false;

            Lighting.AddLight(dust.position, new Color(51, 185, 131).ToVector3() * 0.6f);

            return false;
        }

        public override bool MidUpdate(Dust dust) => true;

        public override Color? GetAlpha(Dust dust, Color lightColor)
            => Color.White;

    }
}