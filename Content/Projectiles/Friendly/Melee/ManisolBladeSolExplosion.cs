using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ManisolBladeSolExplosion : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 4;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize(128, 128);
            Projectile.knockBack = 8f;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            int impactDustCount = Main.rand.Next(250, 280);
            for (int i = 0; i < impactDustCount; i++)
            {
                int dist = 20;
                Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                Vector2 velocity = (Projectile.Center - dustPosition).SafeNormalize(default) * -8f * distFactor;
                Particle.Create<DustParticle>((p =>
                {
                    p.DustType = DustID.SolarFlare;
                    p.Position = dustPosition;
                    p.Velocity = velocity;
                    p.Scale = new Vector2(Main.rand.NextFloat(0.6f, 1.2f));
                    p.NoGravity = true;
                    p.NormalUpdate = true;
                }));
            }

            int flameParticleCount = Main.rand.Next(10, 20);
            for (int i = 0; i < flameParticleCount; i++)
            {
                int dist = 40;
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                float distFactor = (Vector2.DistanceSquared(Projectile.Center, position) / (dist * dist));
                Vector2 velocity = (Projectile.Center - position).SafeNormalize(default) * -140f;
                Particle.Create(ParticleOrchestraType.AshTreeShake, position, velocity);
            }

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = new(0.4f);
                p.ScaleVelocity = new(0.12f);
                p.Color = new Color(255, 164, 57);
            });

            Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = new Color(255, 164, 57).WithOpacity(0.1f) * 0.4f;
                p.Scale = new(1f);
                p.NumberOfInnerReplicas = 6;
                p.ReplicaScalingFactor = 1.6f;
            });
        }
    }
}
