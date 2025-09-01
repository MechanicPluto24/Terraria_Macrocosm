using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class NebulaStaffMeteor : ModProjectile
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

            if (Projectile.owner == Main.myPlayer)
            {
                int impactDustCount = Main.rand.Next(10, 16);
                for (int i = 0; i < impactDustCount; i++)
                {
                    int dist = 100;
                    Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                    Vector2 velocity = (Projectile.Center - dustPosition).SafeNormalize(default) * -10f;

                    Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), dustPosition, velocity, ModContent.ProjectileType<NebulaRemnantProjectile>(), (int)(Projectile.damage / 8), 0, Projectile.owner);
                }
            }
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
                    DustID.UndergroundHallowedEnemies,
                    0f,
                    0f,
                    Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
                );

                dust.noGravity = true;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
        }

        public void ImpactEffects()
        {
            int impactDustCount = Main.rand.Next(450, 480);
            for (int i = 0; i < impactDustCount; i++)
            {
                int dist = 250;
                Vector2 offset = Main.rand.NextVector2Circular(dist, dist);
                Vector2 dustPosition = Projectile.Center + offset;
                float distFactor = 0.2f + 0.8f * (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                Vector2 velocity = -offset * 0.04f * distFactor;
                Particle.Create<DustParticle>((p =>
                {
                    p.DustType = DustID.UndergroundHallowedEnemies;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Acceleration = velocity * 0.2f;
                    p.Scale = new(Main.rand.NextFloat(0.6f, 1.8f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                }));
            }

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = new(0.2f);
                p.ScaleVelocity = new(0.3f);
                p.Color = new Color(255, 72, 255);
            });

            Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = new Color(255, 72, 255).WithOpacity(0.1f) * 0.4f;
                p.Scale = new(1f);
                p.NumberOfInnerReplicas = 6;
                p.ReplicaScalingFactor = 2.6f;
            });
        }
    }
}
