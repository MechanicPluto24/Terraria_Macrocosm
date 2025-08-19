using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class IlmeniteExplosion : ModProjectile
{
    public override string Texture => Macrocosm.EmptyTexPath;

    public float Strength
    {
        get => MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
        set => Projectile.ai[0] = MathHelper.Clamp(value, 0f, 1f);
    }

    public override void SetStaticDefaults()
    {
        ProjectileSets.HitsTiles[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.Size = new Vector2(50, 50);
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;

        Projectile.CritChance = 16;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.Size *= 1f + (0.5f * Strength);
    }

    private bool spawned;
    public override void AI()
    {
        if (!spawned)
        {
            Particle.Create<TintableFlash>(p =>
            {
                p.Position = Projectile.Center;
                p.Scale = new(0.2f);
                p.ScaleVelocity = new(0.2f);
                p.Color = Main.rand.NextBool() ? new(188, 89, 134) : new(33, 188, 190);
            });

            Lighting.AddLight(Projectile.Center, new Color(188, 89, 134).ToVector3() * 4f);
            Lighting.AddLight(Projectile.Center, new Color(33, 188, 190).ToVector3() * 4f);

            spawned = true;
        }

        Lighting.AddLight(Projectile.Center, new Color(188, 89, 134).ToVector3());
        Lighting.AddLight(Projectile.Center, new Color(33, 188, 190).ToVector3());

        for (int i = 0; i < 30 * Strength; i++)
        {
            Vector2 edgeOffset = Main.rand.NextVector2CircularEdge(Projectile.width * 1.2f, Projectile.height * 1.2f);
            float u = Main.rand.NextFloat();
            float factor = 1f - (float)Math.Sqrt(1f - u);
            Vector2 offset = edgeOffset * factor;
            Vector2 position = Projectile.Center + offset;

            Particle.Create<LunarRustStar>((p) =>
            {
                p.Position = position;
                p.Velocity = Vector2.Zero;
                p.Rotation = Utility.RandomRotation();
                p.Scale = new Vector2(1f * (0.6f + Strength * 0.15f)) * Main.rand.NextFloat(0.5f, 1.2f);
            });
        }
    }

    public void OnHit(Vector2 hitVelocity)
    {
        Projectile.timeLeft += 15;

        for (int i = 0; i < (int)(Strength * 6); i++)
        {
            Vector2 targetPos = Projectile.Center + (hitVelocity.RotatedByRandom(MathHelper.Pi / 4) * 100);

            Projectile.NewProjectileDirect(
                Projectile.GetSource_FromAI(),
                Projectile.Center,
                hitVelocity,
                ModContent.ProjectileType<IlmeniteProjectileDeflected>(),
                Projectile.damage / 3,
                knockback: 2f,
                owner: Main.myPlayer,
                ai0: i % 2,
                ai1: targetPos.X,
                ai2: targetPos.Y
            );
        }

        Particle.Create<TintableFlash>(p =>
        {
            p.Position = Projectile.Center;
            p.Scale = new(0.2f);
            p.ScaleVelocity = new(0.2f);
            p.Color = Main.rand.NextBool() ? new(188, 89, 134) : new(33, 188, 190);
        });

        for (int i = 0; i < 10 * Strength; i++)
        {
            Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
            Particle.Create<LunarRustStar>((p) =>
            {
                p.Position = position;
                p.Velocity = Vector2.Zero;
                p.Rotation = Utility.RandomRotation();
                p.Scale = new Vector2(1f * (0.6f + Strength * 0.15f)) * Main.rand.NextFloat(0.5f, 1.2f);
                p.Opacity = Main.rand.NextFloat();
            });
        }
    }


    public void OnHit()
    {
    }


    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
