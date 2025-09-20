using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles;

public class TintableFire : Particle
{
    private static Asset<Texture2D> flare;
    public override int TrailCacheLength => 15;
    public override string Texture => Macrocosm.EmptyTexPath;
    public bool FadeIn { get; set; }
    public bool FadeOut { get; set; }

    public override void SetDefaults()
    {
        FadeIn = false;
        FadeOut = true;
        DrawLayer = ParticleDrawLayer.BeforeNPCs;
    }

    public override void OnSpawn()
    {
        if (FadeOut)
            alpha = 255;

        if (FadeIn)
            alpha = 0;
    }

    public int FadeInSpeed = 1;
    public int FadeOutSpeed = 7;
    public int TargetAlpha = 255;

    private float alpha = 255;
    private bool fadedIn = false;

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        flare ??= ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Flame4");
        spriteBatch.Draw(flare.Value, Position - screenPosition, GetFrame(), Color * ((float)alpha / 255f), Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
    }

    public override void AI()
    {
        Scale *= 0.98f;
        Velocity.Y = -0.8f;
        Rotation += 0.01f;
        if (FadeIn && FadeOut)
        {
            if (!fadedIn)
            {
                if (alpha < TargetAlpha)
                    alpha += FadeInSpeed;
                else
                    fadedIn = true;
            }
            else if (alpha > 0)
            {
                alpha -= FadeOutSpeed;
            }
        }
        else
        {
            if (FadeIn && alpha < TargetAlpha)
                alpha += FadeInSpeed;

            if (FadeOut && alpha > 0)
                alpha -= FadeOutSpeed;
        }

        alpha = (int)MathHelper.Clamp(alpha, 0, 255);
        if ((fadedIn && alpha <= 0))
            Kill();
    }
}
