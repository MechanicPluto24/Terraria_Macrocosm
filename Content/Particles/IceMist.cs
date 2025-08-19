using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Particles;

public class IceMist : Particle
{
    public override string Texture => Macrocosm.FancyTexturesPath + "Smoke1";

    public float Opacity { get; set; }
    public float ExpansionRate { get; set; }
    public bool FadeIn { get; set; }

    private bool fadedIn;

    public override void SetDefaults()
    {
        Color = new Color(56, 188, 173, 0);
        ExpansionRate = -0.008f;
        Opacity = 0.3f;
        FadeIn = false;
        fadedIn = false;
    }

    public override void OnSpawn()
    {
    }

    public override void AI()
    {
        if (!fadedIn)
        {
            Scale -= new Vector2(ExpansionRate / 3);
            Opacity += 0.05f;
            if (Opacity >= 1f)
                fadedIn = true;
        }
        else
        {
            Scale += new Vector2(ExpansionRate / 3);
            if (Opacity > 0f)
                Opacity -= 0.012f;
        }

        if (Scale.X < 0.1f || (Opacity <= 0 && fadedIn))
            Kill();
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        spriteBatch.Draw(TextureAsset.Value, Position - screenPosition, GetFrame(), Utility.Colorize(Color, lightColor).WithAlpha(Color.A) * Opacity, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
    }
}
