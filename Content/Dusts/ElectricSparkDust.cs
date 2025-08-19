using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts;

public class ElectricSparkDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.rotation += dust.dustIndex % 2 == 0 ? 0.2f : -0.2f;

        float scale = dust.scale;
        if (scale > 1f)
            scale = 1f;

        if (!dust.noLight)
            Lighting.AddLight(dust.position, dust.color.ToVector3() * scale);

        if (dust.noGravity)
        {
            dust.velocity *= 0.93f;
            if (dust.fadeIn == 0f)
                dust.scale += 0.0025f;
        }
        else
        {
            dust.velocity.Y += 0.035f;
        }

        dust.velocity *= new Vector2(0.97f, 0.99f);

        if (dust.customData != null && dust.customData is Player player)
            dust.position += player.position - player.oldPosition;

        dust.scale -= 0.01f;
        if (dust.scale < 0.1f)
            dust.active = false;

        return false;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color.WithAlpha((byte)dust.alpha);

    public override bool PreDraw(Dust dust)
    {
        float count = Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y) * 3f;
        if (count > 10f)
            count = 10f;

        for (int n = 0; n < count; n++)
        {
            Vector2 trailPosition = dust.position - dust.velocity * n;
            float scale = dust.scale * (1f - n / 10f);
            Color color = Lighting.GetColor((int)(dust.position.X + 4.0) / 16, (int)(dust.position.Y + 4.0) / 16);
            Main.spriteBatch.Draw(dust.GetTexture(), trailPosition - Main.screenPosition, dust.frame, dust.GetAlpha(color), dust.rotation, new Vector2(4f, 4f), scale, SpriteEffects.None, 0f);
        }

        return true;
    }
}
