using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class StardustStaffMeteor : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Redemption.AddElementToItem(Type, Redemption.ElementID.Explosive);
        Redemption.AddElementToItem(Type, Redemption.ElementID.Arcane);
        Redemption.AddElementToItem(Type, Redemption.ElementID.Celestial);
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.width = 64;
        Projectile.height = 64;
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.netMode != NetmodeID.Server)
            ImpactEffects();
    }

    public override void AI()
    {
        float dustScaleMin = 1f;
        float dustScaleMax = 1.6f;

        if (Main.rand.NextBool(1))
        {
            Dust dust = Dust.NewDustDirect(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                Main.rand.NextBool() ? DustID.YellowStarDust : DustID.DungeonWater,
                0f,
                0f,
                Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
            );

            dust.noGravity = true;
        }
        Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //Check to see if there even IS a worm.
        if (Projectile.owner == Main.myPlayer)
        {
            Player player = Main.player[Projectile.owner];
            List<Projectile> worms = new();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.owner == Projectile.owner && p.type == ModContent.ProjectileType<StardustWormProjectile>() && p.active)
                    worms.Add(p);
            }

            if (worms.Count == 0)
            {
                Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StardustWormProjectile>(), (int)(Projectile.damage / 8), 0, Main.myPlayer, 0f);
                p1.timeLeft = 1000;
                Projectile p2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StardustWormProjectile>(), (int)(Projectile.damage / 8), 0, Main.myPlayer, 1f);
                p2.timeLeft = 1100;
            }
            else
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StardustWormProjectile>(), (int)(Projectile.damage / 8), 0, Main.myPlayer, worms.Count);
                p.timeLeft = 1000;
            }
        }
    }

    public void ImpactEffects()
    {
        int impactDustCount = Main.rand.Next(450, 480);
        for (int i = 0; i < impactDustCount; i++)
        {
            int dist = 80;
            Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
            float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
            Vector2 velocity = (Projectile.Center - dustPosition).SafeNormalize(default) * -8f;
            Particle.Create<DustParticle>((p =>
            {
                p.DustType = Main.rand.NextBool() ? DustID.YellowStarDust : DustID.DungeonWater;
                p.Position = dustPosition;
                p.Velocity = velocity;
                p.Scale = new Vector2(Main.rand.NextFloat(1.8f, 2f)) * (1f - distFactor);
                p.NoGravity = true;
                p.NormalUpdate = true;
            }));
        }

        Particle.Create<TintableFlash>((p) =>
        {
            p.Position = Projectile.Center;
            p.Scale = new(0.8f);
            p.ScaleVelocity = new(0.2f);
            p.Color = new Color(116, 164, 255).WithOpacity(1f);
        });

        Particle.Create<TintableExplosion>(p =>
        {
            p.Position = Projectile.Center;
            p.Color = new Color(116, 164, 255).WithOpacity(0.1f) * 0.4f;
            p.Scale = new(1.5f);
            p.NumberOfInnerReplicas = 8;
            p.ReplicaScalingFactor = 1.4f;
        });


        Particle.Create<TintableExplosion>(p =>
        {
            p.Position = Projectile.Center;
            p.Color = new Color(252, 241, 69).WithOpacity(0.1f) * 0.4f;
            p.Scale = new(1.2f);
            p.NumberOfInnerReplicas = 6;
            p.ReplicaScalingFactor = 1.2f;
            p.Rotation = MathHelper.PiOver2;
        });

        for (int i = 0; i < 45; i++)
        {
            Vector2 position = Projectile.Center + new Vector2(120).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
            Vector2 velocity = -new Vector2(10).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
            Particle.Create(ParticleOrchestraType.StardustPunch, position, velocity);
        }
    }
}
