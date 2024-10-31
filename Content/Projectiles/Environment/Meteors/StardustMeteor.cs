using Macrocosm.Common.Bases.Projectiles;
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
    public class StardustMeteor : BaseMeteor
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
                    Main.rand.NextBool() ? DustID.YellowStarDust : DustID.DungeonWater,
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
                int dist = 160;
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

        public override void SpawnItems()
        {
            int type = ModContent.ItemType<StardustChunk>();
            int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, type);
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
        }
    }
}
