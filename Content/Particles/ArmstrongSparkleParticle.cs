using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles;

public class ArmstrongSparkleParticle : Particle
{
    public override int MaxPoolCount => 50;
    public override string Texture => Macrocosm.FancyTexturesPath + "Star4";

    public override void SetDefaults()
    {
        TimeToLive = 60;
        Scale = new Vector2(0.1f);
        Rotation = 0f;
        Color = Color.White;
        FadeInNormalizedTime = 0.1f;
        FadeOutNormalizedTime = 0.8f;
    }

    public override void AI()
    {
        Velocity *= 0.95f;
        Scale *= 0.99f;
        if (Scale.LengthSquared() <= 0.001f)
            Kill();
    }

    public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        spriteBatch.Draw(TextureAsset.Value, Position - screenPosition, null, Color * Scale.Length() * 10f * FadeFactor, 0f, TextureAsset.Size() / 2f, Scale * 0.85f, SpriteEffects.None, 0f);
        return false;
    }
}
