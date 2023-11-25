using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class PlasmaBallDust : ModDust
    {

        float vanishSpeed;

        public override void OnSpawn(Dust dust)
        {
            dust.scale = Main.rand.NextFloat(1, 1.35f);
            vanishSpeed = Main.rand.NextFloat(0.04f, 0.01f);
        }

        public override bool Update(Dust dust)
        {
            object owner;
            bool hasOwner = false;
            bool ownerActive = false;

            if (dust.customData is Projectile)
            {
                owner = dust.customData as Projectile;
                hasOwner = true;
                ownerActive = (owner as Projectile).active;
            }
            else if (dust.customData is Particle)
            {
                owner = dust.customData as Particle;
                hasOwner = true;
                ownerActive = (owner as Particle).Active;
            }

            dust.position += dust.velocity;

            if (!hasOwner || ownerActive == false)
                dust.scale -= vanishSpeed * 3f;
            else
                dust.scale -= vanishSpeed * 0.7f;

            if (dust.scale < 0.2f)
                dust.active = false;

            Lighting.AddLight(dust.position, new Vector3(0.407f, 1f, 1f) * dust.scale * 0.2f);

            return false;
        }


        public override bool MidUpdate(Dust dust) => true;


        public override Color? GetAlpha(Dust dust, Color lightColor)
            => Color.White;
    }
}