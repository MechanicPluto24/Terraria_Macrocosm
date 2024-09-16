using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.GrabBags;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Environment.Debris;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class MoonMeteorMedium : BaseMeteor
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 48;
            Projectile.height = 48;

            ScreenshakeMaxDist = 110f * 16f;
            ScreenshakeIntensity = 75f;

            RotationMultiplier = 0.01f;
            BlastRadiusMultiplier = 3.5f;
        }

        public override void MeteorAI()
        {
            float DustScaleMin = 1f;
            float DustScaleMax = 1.4f;

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                        new Vector2(Projectile.position.X, Projectile.position.Y),
                        Projectile.width,
                        Projectile.height,
                        ModContent.DustType<RegolithDust>(),
                        0f,
                        0f,
                        Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                    );

                dust.noGravity = true;
            }
        }

        public override void ImpactEffects()
        {
            int ImpactDustCount = Main.rand.Next(100, 120);
            Vector2 ImpactDustSpeed = new Vector2(2f, 7.5f);
            float DustScaleMin = 1f;
            float DustScaleMax = 1.4f;

            int DebrisType = ModContent.ProjectileType<RegolithDebris>();
            int DebrisCount = Main.rand.Next(4, 6);
            Vector2 DebrisVelocity = new Vector2(0.5f, 0.7f);

            for (int i = 0; i < ImpactDustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    new Vector2(Projectile.Center.X, Projectile.Center.Y),
                    Projectile.width,
                    Projectile.height,
                    ModContent.DustType<RegolithDust>(),
                    Main.rand.NextFloat(-ImpactDustSpeed.X, ImpactDustSpeed.X),
                    Main.rand.NextFloat(0f, -ImpactDustSpeed.Y),
                    Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                );

                dust.noGravity = true;
            }

            var explosion = Particle.CreateParticle<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.DrawColor = (new Color(120, 120, 120)).WithOpacity(0.8f);
                p.Scale = new(1.5f);
                p.NumberOfInnerReplicas = 10;
                p.ReplicaScalingFactor = 0.4f;
            });

            for (int i = 0; i < DebrisCount; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                new Vector2(DebrisVelocity.X * Main.rand.NextFloat(-6f, 6f), DebrisVelocity.Y * Main.rand.NextFloat(-4f, -1f)),
                DebrisType, 0, 0f, 255);
            }
        }

        public override void SpawnItems()
        {
            if (Main.rand.NextBool(3))
            {
                int type = ModContent.ItemType<MeteoricChunk>();
                Vector2 position = new Vector2(Projectile.position.X + Projectile.width / 2, Projectile.position.Y - Projectile.height);
                int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), position, new Vector2(Projectile.width, Projectile.height), type);
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
            }

        }
    }
}
