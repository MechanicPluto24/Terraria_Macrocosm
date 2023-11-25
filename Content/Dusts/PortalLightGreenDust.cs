using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class PortalLightGreenDust : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor) => Color.White;

        public override void OnSpawn(Dust dust)
        {
            dust.noLight = false;
            dust.color = Color.White;
            dust.alpha = 255;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 2) * 10, 10, 10);
        }

        public override bool Update(Dust dust)
        {
            if (!dust.noGravity)
                dust.velocity.Y += 0.05f;

            if (dust.customData != null && dust.customData is NPC npc)
            {
                dust.position += npc.position - npc.oldPos[1];
            }
            else if (dust.customData != null && dust.customData is Player player)
            {
                dust.position += player.position - player.oldPosition;
            }
            else if (dust.customData != null && dust.customData is Vector2)
            {
                Vector2 vector = (Vector2)dust.customData - dust.position;
                if (vector != Vector2.Zero)
                    vector.Normalize();

                dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;
            }

            if (dust.alpha > 0)
                dust.alpha -= 24;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, new Color(30, 255, 105).ToVector3() * 0.6f);

            dust.position += dust.velocity;
            dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
            dust.scale -= 0.08f;

            if (dust.scale <= 0f)
                dust.active = false;

            return false;
        }
    }
}