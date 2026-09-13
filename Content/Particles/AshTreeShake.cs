using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;

namespace Macrocosm.Content.Particles;

public static class AshTreeShake
{
    private static readonly ParticlePool<PrettySparkleParticle> ashSparkles = new(200, () => new PrettySparkleParticle());

    // Retains the orange sparkle burst removed from vanilla's particle orchestra in 1.4.5.
    public static void Spawn(Vector2 position)
    {
        if (Main.dedServ)
            return;

        float lifetime = 10f + 20f * Main.rand.NextFloat();
        float scale = 0.2f + 0.4f * Main.rand.NextFloat();
        Color tint = Main.hslToRgb(Main.rand.NextFloat() * 0.1f + 0.06f, 1f, 0.5f);
        tint.A /= 2;
        tint *= Main.rand.NextFloat() * 0.3f + 0.7f;

        for (int layer = 0; layer < 2; layer++)
        {
            for (int side = 0; side < 2; side++)
            {
                Vector2 direction = (MathHelper.Pi * side).ToRotationVector2() * 4f;
                PrettySparkleParticle sparkle = ashSparkles.RequestParticle();
                sparkle.ColorTint = layer == 0 ? tint : new Color(1f, 0.4f, 0.2f, 1f);
                sparkle.LocalPosition = position - direction * lifetime * 0.25f;
                sparkle.Rotation = direction.ToRotation();
                sparkle.Scale = new Vector2(4f, 1f) * (layer == 0 ? 1.1f : 0.7f) * scale;
                sparkle.FadeInNormalizedTime = 0.000005f;
                sparkle.FadeOutNormalizedTime = 0.95f;
                sparkle.TimeToLive = lifetime;
                sparkle.FadeOutEnd = lifetime;
                sparkle.FadeInEnd = lifetime / 2f;
                sparkle.FadeOutStart = lifetime / 2f;
                if (layer == 0)
                    sparkle.AdditiveAmount = 0.35f;
                sparkle.Velocity = direction * 0.05f;
                sparkle.DrawVerticalAxis = false;
                if (side == 1)
                {
                    sparkle.Scale *= 1.5f;
                    sparkle.Velocity *= 1.5f;
                    sparkle.LocalPosition -= sparkle.Velocity * 4f;
                }
                Main.ParticleSystem_World_OverPlayers.Add(sparkle);

                if (layer == 1)
                {
                    for (int sign = 1; sign >= -1; sign -= 2)
                    {
                        Dust dust = Dust.NewDustPerfect(position, DustID.Torch,
                            sign * direction.RotatedBy(Main.rand.NextFloatDirection() * MathHelper.TwoPi * 0.025f) * Main.rand.NextFloat());
                        dust.noGravity = true;
                        dust.scale = 1.4f;
                    }
                }
            }
        }
    }
}
