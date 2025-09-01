using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.GrabBags;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Meteors;

public class NebulaMeteor : BaseMeteor
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

        // Nebula Dusts: 71, 72, 73
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
    }

    public override void ImpactEffects(int collisionTileType)
    {
        int impactDustCount = Main.rand.Next(450, 480);
        for (int i = 0; i < impactDustCount; i++)
        {
            int dist = 300;
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
                p.Scale = new(Main.rand.NextFloat(0.6f, 2.4f));
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

    public override void SpawnItems()
    {
        int type = ModContent.ItemType<NebulaChunk>();
        int itemIdx = Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Center, type);
        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIdx, 1f);
    }
}
