using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.GrabBags;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class SolarMeteor : BaseMeteor
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
                    DustID.SolarFlare,
                    0f,
                    0f,
                    Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                );

                dust.noGravity = true;
            }
        }

        public override void ImpactEffects()
        {
            int impactDustCount = Main.rand.Next(550, 580); 
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
                    p.Scale = new Vector2(Main.rand.NextFloat(1.2f, 2f)) ;
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                }));
            }

            int flameParticleCount = Main.rand.Next(20, 30);
            for (int i = 0; i < flameParticleCount; i++)
            {
                int dist = 224;
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                float distFactor = (Vector2.DistanceSquared(Projectile.Center, position) / (dist * dist));
                Vector2 velocity = (Projectile.Center - position).SafeNormalize(default) * -140f;
                Particle.Create(ParticleOrchestraType.AshTreeShake, position, velocity);
            }


            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = new(0.8f);
                p.ScaleVelocity = new(0.2f);
                p.Color = CelestialDisco.SolarColor.WithOpacity(1f);
            });
        }

        public override void SpawnItems()
        {
            int type = ModContent.ItemType<SolarChunk>();
            int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center,  type);
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
        }
    }
}
