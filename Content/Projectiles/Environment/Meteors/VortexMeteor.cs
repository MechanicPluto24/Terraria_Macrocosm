using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing;
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

            ScreenshakeMaxDist = 140f * 16f;
            ScreenshakeIntensity = 100f;

            RotationMultiplier = 0.01f;
            BlastRadius = 224;
        }

        public override void MeteorAI()
        {
            float DustScaleMin = 1f;
            float DustScaleMax = 1.6f;

            if (Main.rand.NextBool(1))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Vortex,
                    0f,
                    0f,
                    Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                );

                dust.noGravity = true;
            }
        }

        public override void ImpactEffects()
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
            }

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = new(0f);
                p.ScaleVelocity = new(0.2f);
                p.Color = CelestialDisco.VortexColor.WithOpacity(1f);
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
