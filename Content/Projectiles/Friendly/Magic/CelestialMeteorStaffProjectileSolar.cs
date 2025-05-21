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
    public class CelestialMeteorStaffProjectileSolar : ModProjectile
    {
        public override string Texture => "Macrocosm/Content/Projectiles/Environment/Meteors/SolarMeteor";

        public override void SetStaticDefaults()
        {
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Explosive);
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Fire);
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Celestial);
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
                    DustID.SolarFlare,
                    0f,
                    0f,
                    Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
                );

                dust.noGravity = true;
            }
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SolarGlob>(), (int)(Projectile.damage / 3), 0, Projectile.owner);
            return true;
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
                    p.DustType = DustID.SolarFlare;
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
