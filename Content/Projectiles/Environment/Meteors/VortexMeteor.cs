using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.GrabBags;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class VortexMeteor : BaseMeteor
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 64;
            Projectile.height = 64;

            screenshakeMaxDist = 140f * 16f;
            screenshakeIntensity = 100f;

            rotationMultiplier = 0.01f;
            blastRadius = 224;
        }

        public override void MeteorAI()
        {
            float dustScaleMin = 1f;
            float dustScaleMax = 1.6f;

            if (Main.rand.NextBool(1))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Vortex,
                    0f,
                    0f,
                    Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
                );

                dust.noGravity = true;
            }
        }

        public override void ImpactEffects(int collisionTileType)
        {
            int impactDustCount = Main.rand.Next(450, 480);
            for (int i = 0; i < impactDustCount; i++)
            {
                int dist = 80;
                Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                float radians = (Projectile.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
                Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
                Particle.Create<DustParticle>((p =>
                {
                    p.DustType = DustID.Vortex;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Acceleration = velocity * 80f;
                    p.Scale = new(Main.rand.NextFloat(1.8f, 2f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                }));


                dist = 40;
                dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                radians = (Projectile.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
                velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);

                Particle.Create<PortalSwirl>((p =>
                {
                    p.Position = dustPosition;
                    p.Scale = new(Main.rand.NextFloat(0.2f, 0.3f));
                    p.Velocity = velocity;
                    p.Acceleration = velocity * 40f;
                    p.TimeToLive = 30;
                    p.FadeInNormalizedTime = 0f;
                    p.FadeOutNormalizedTime = 0.9f;
                    p.Color = new Color(68, 255, 255);
                }));
            }

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center;
                p.Scale = new(0f);
                p.ScaleVelocity = new(0.2f);
                p.Color = new Color(68, 255, 255);
            });

            Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = new Color(68, 255, 255).WithOpacity(0.1f) * 0.4f;
                p.Scale = new(1f);
                p.NumberOfInnerReplicas = 6;
                p.ReplicaScalingFactor = 2.6f;
            });
        }

        public override void SpawnItems()
        {
            int type = ModContent.ItemType<VortexChunk>();
            int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, type);
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
        }
    }
}
