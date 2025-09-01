using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Macrocosm.Content.Particles;

public class PortalSwirl : Particle
{
    public override string Texture => Macrocosm.FancyTexturesPath + "Swirl1";
    public Vector2 TargetCenter { get; set; }

    public override void SetDefaults()
    {
        TimeToLive = 150;
        TargetCenter = default;
    }

    public override void OnSpawn()
    {
        if (TargetCenter != default)
        {
            float speed = Velocity.Length();
            Vector2 toCenter = (TargetCenter - Position).SafeNormalize(Vector2.One);
            Vector2 tangential = new(-toCenter.Y, toCenter.X);
            Velocity = tangential * speed;
        }
    }

    public override void AI()
    {
        if (Scale.X < 0.1f)
            Kill();

        if (Velocity.LengthSquared() < 0.5f)
            Kill();

        if (TargetCenter != default)
        {
            Vector2 toCenter = (TargetCenter - Position).SafeNormalize(Vector2.One) * Velocity.Length();
            Velocity = Vector2.Lerp(Velocity, toCenter, 0.3f);
            Velocity *= 0.995f;
        }
    }

    public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
    {
        float speed = Velocity.LengthSquared();

        Color color = (Color.Lerp(Color.White, Color, speed) * 0.9f).WithOpacity(1f);

        Main.spriteBatch.Draw(
            TextureAsset.Value,
            Position - screenPosition,
            null,
            color * FadeFactor,
            Velocity.ToRotation(),
            TextureAsset.Size() * 0.5f,
            new Vector2(Math.Clamp(speed, 0, 2), Math.Clamp(speed, 0, 0.5f)) * 0.15f * Scale,
            SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,
            0
        );

        return false;
    }
}