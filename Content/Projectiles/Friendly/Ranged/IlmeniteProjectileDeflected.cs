using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class IlmeniteProjectileDeflected : ModProjectile
{
    public override string Texture => Macrocosm.EmptyTexPath;

    private Vector2 Target
    {
        get
        {
            return new(
                Projectile.ai[1],
                Projectile.ai[2]
            );
        }
        set
        {
            Projectile.ai[1] = value.X;
            Projectile.ai[2] = value.Y;
        }
    }

    float speed;
    float trailMultiplier = 0f;
    int colourLerpProg = 0;
    public Color colour1 = new(188, 89, 134);
    public Color colour2 = new(33, 188, 190);

    public override void SetStaticDefaults()
    {
        ProjectileSets.HitsTiles[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 360;
        Projectile.extraUpdates = 1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void OnSpawn(IEntitySource source)
    {
    }

    private bool spawned;
    private bool reachedTarget;
    public override void AI()
    {
        if (!spawned)
        {
            speed = Projectile.velocity.Length();
            spawned = true;
        }

        if (!reachedTarget)
        {
            Vector2 direction = Projectile.DirectionTo(Target).SafeNormalize(Vector2.Zero);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * speed, 0.02f);

            if ((Projectile.Center - Target).LengthSquared() < 100 * 100)
                reachedTarget = true;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (trailMultiplier < 1f + (0.15f * Projectile.extraUpdates))
            trailMultiplier += 0.03f * (0.2f + Projectile.ai[0] * 0.9f);

        if (Projectile.ai[0] == 0)
        {
            Lighting.AddLight(Projectile.Center, colour1.ToVector3());
        }
        else if (Projectile.ai[0] == 1)
        {
            Lighting.AddLight(Projectile.Center, colour2.ToVector3() * 1.25f);
        }
        else
        {
            Lighting.AddLight(Projectile.Center, Color.Lerp(colour1, colour2, MathF.Pow(MathF.Cos(colourLerpProg / 10f), 3)).ToVector3() * 1.5f);
            colourLerpProg++;
        }
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item10);

        for (int i = 0; i < 15; i++)
        {
            Particle.Create<TintableSpark>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity;
                p.Velocity = Main.rand.NextVector2Circular(4, 4) * Main.rand.NextFloat();
                p.Scale = new(6f, Main.rand.NextFloat(2.5f, 3.5f));
                p.Color = Main.rand.NextBool() ? colour1 : colour2;
            });
        }

        float count = Projectile.oldVelocity.Length() * trailMultiplier;
        for (int i = 1; i < count; i++)
        {
            Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * i * 0.4f;
            for (int j = 0; j < 2; j++)
            {
                Particle.Create<TintableSpark>((p) =>
                {
                    p.Position = trailPosition;
                    p.Velocity = Vector2.Zero;
                    p.Scale = new(Main.rand.NextFloat(1f, 2f) * (1f - i / count));
                    p.Color = Main.rand.NextBool() ? colour1 : colour2;
                });
            }
        }
    }

    SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        var spriteBatch = Main.spriteBatch;

        state.SaveState(spriteBatch);
        spriteBatch.End();
        spriteBatch.Begin(BlendState.Additive, state);

        float count = Projectile.velocity.LengthSquared() * trailMultiplier;

        var color = Projectile.ai[0] switch
        {
            0 => colour1,
            1 => colour2,
            _ => Color.Lerp(colour1, colour2, MathF.Pow(MathF.Cos(colourLerpProg / 10f), 3)) * 1.5f,
        };

        for (int n = 1; n < count; n++)
        {
            Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 0.4f;
            color *= 1f - (float)n / count;
            Utility.DrawStar(trailPosition - Main.screenPosition, 1, color, Projectile.scale * (0.6f + Projectile.ai[0] * 0.15f), Projectile.rotation, entity: true);
        }

        spriteBatch.End();
        spriteBatch.Begin(state);

        return false;
    }
}
