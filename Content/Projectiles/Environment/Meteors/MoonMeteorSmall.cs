﻿using Macrocosm.Common.Bases.Projectiles;
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
    public class MoonMeteorSmall : BaseMeteor
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 32;
            Projectile.height = 32;

            ScreenshakeMaxDist = 90f * 16f;
            ScreenshakeIntensity = 50f;

            RotationMultiplier = 0.01f;
            BlastRadius = 112;
        }

        public override void MeteorAI()
        {
            float DustScaleMin = 1f;
            float DustScaleMax = 1.2f;

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
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
            int ImpactDustCount = Main.rand.Next(60, 80);
            Vector2 ImpactDustSpeed = new Vector2(1f, 5f);
            float DustScaleMin = 1f;
            float DustScaleMax = 1.2f;

            int DebrisType = ModContent.ProjectileType<RegolithDebris>();
            int DebrisCount = Main.rand.Next(2, 4);
            Vector2 DebrisVelocity = new Vector2(0.5f, 0.6f);

            for (int i = 0; i < ImpactDustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    ModContent.DustType<RegolithDust>(),
                    Main.rand.NextFloat(-ImpactDustSpeed.X, ImpactDustSpeed.X),
                    Main.rand.NextFloat(0f, -ImpactDustSpeed.Y),
                    Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax)
                );

                dust.noGravity = true;
            }

            var explosion = Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = (new Color(120, 120, 120)).WithOpacity(0.8f);
                p.Scale = new(1.25f);
                p.NumberOfInnerReplicas = 8;
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
                int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center,  type);
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
            }
        }
    }
}
