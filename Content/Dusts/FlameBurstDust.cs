using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{

    /// <summary>
    /// Using vanilla texture and adapted from DustID.FrameBurst (270)
    /// </summary>
    public class FlamingMeteorDust : ModDust
    {
        public override string Texture => null;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Utility.VanillaDustFrame(DustID.Torch);
        }

        public override bool Update(Dust dust)
        {

            dust.velocity *= 1.0050251f;
            dust.scale *= 0.998f;
            dust.rotation = 0f;

            if (Collision.SolidCollision(dust.position - Vector2.One * 5f, 10, 10) && dust.fadeIn == 0f)
            {
                dust.scale *= 0.95f;
            }
            else
            {
                dust.velocity.Y = (float)Math.Sin(dust.position.X * 0.0043982295f) * 2f;
                dust.velocity.Y -= 3f;
                dust.velocity.Y /= 20f;
            }

            if (dust.scale <= 0.4f)
            {
                dust.active = false;
            }

            dust.position += dust.velocity;

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color((int)lightColor.R / 2 + 127, (int)lightColor.G / 2 + 127, (int)lightColor.B / 2 + 127, 25);
        }
    }
}