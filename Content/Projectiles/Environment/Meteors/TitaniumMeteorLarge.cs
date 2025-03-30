using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.GrabBags;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Environment.Debris;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors
{
    public class TitaniumMeteorLarge : BaseMeteor
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

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(
                        new Vector2(Projectile.position.X, Projectile.position.Y),
                        Projectile.width,
                        Projectile.height,
                        DustID.Titanium,
                        0f,
                        0f,
                        Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
                    );

                dust.noGravity = true;
            }
        }

        public override void ImpactEffects(int collisionTileType)
        {
            int impactDustCount = Main.rand.Next(140, 160);
            Vector2 impactDustSpeed = new Vector2(3f, 10f);
            float dustScaleMin = 1f;
            float dustScaleMax = 1.6f;

            int debrisType = ModContent.ProjectileType<RegolithDebris>();
            int debrisCount = Main.rand.Next(6, 8);
            Vector2 debrisVelocity = new Vector2(0.5f, 0.8f);

            for (int i = 0; i < impactDustCount; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Titanium,
                    Main.rand.NextFloat(-impactDustSpeed.X, impactDustSpeed.X),
                    Main.rand.NextFloat(0f, -impactDustSpeed.Y),
                    Scale: Main.rand.NextFloat(dustScaleMin, dustScaleMax)
                );

                dust.noGravity = false;
            }

            var explosion = Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = (new Color(137, 146, 154) * Lighting.GetColor(Projectile.Center.ToTileCoordinates()).GetBrightness()).WithOpacity(0.8f);
                p.Scale = new(1.7f);
                p.NumberOfInnerReplicas = 12;
                p.ReplicaScalingFactor = 0.4f;
            });

            if (collisionTileType == ModContent.TileType<Regolith>())
            {
                for (int i = 0; i < debrisCount; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                    new Vector2(debrisVelocity.X * Main.rand.NextFloat(-6f, 6f), debrisVelocity.Y * Main.rand.NextFloat(-12f, -6f)),
                    debrisType, 0, 0f, 255);
                }
            }
        }

        public override void SpawnItems()
        {
            // can spawn up to two geodes
            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    int type = ModContent.ItemType<TitaniumChunk>();
                    int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, type);
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
                }
            }
        }
    }
}
