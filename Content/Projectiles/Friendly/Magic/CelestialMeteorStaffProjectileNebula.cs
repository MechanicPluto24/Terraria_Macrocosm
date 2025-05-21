using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class CelestialMeteorStaffProjectileNebula : ModProjectile
    {
        public override string Texture => "Macrocosm/Content/Projectiles/Environment/Meteors/NebulaMeteor";

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
            Projectile.scale = 0.5f;
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
            int impactDustCount = Main.rand.Next(200, 300);
            for (int i = 0; i < impactDustCount; i++)
            {
                int dist = 160;
                Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                Vector2 velocity = (Projectile.Center - dustPosition).SafeNormalize(default) * -14f;
                Particle.Create<DustParticle>((p =>
                {
                    p.DustType = DustID.UndergroundHallowedEnemies;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Scale = new Vector2(Main.rand.NextFloat(1.2f, 2f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                }));
            }
        }
    }
}
