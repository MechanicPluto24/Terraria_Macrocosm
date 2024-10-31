using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class HorusExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.knockBack = 8f;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.hide = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HorusBurn>(), 260, false);
        }



        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            for (int i = 0; i < Main.rand.Next(20, 60); i++)
            {
                int dist = 20;
                Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                Vector2 velocity = (Projectile.Center - dustPosition).SafeNormalize(default) * -8f * distFactor;
                Dust dust = Dust.NewDustDirect(dustPosition, 0, 0, DustID.SolarFlare, velocity.X, velocity.Y, 0, default, Main.rand.NextFloat(0.6f, 1.2f));
                dust.noGravity = true;
            }

            int flameParticleCount = Main.rand.Next(3, 7);
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
                p.Color = new Color(127, 200, 155);
            });

            Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = new Color(255, 164, 57) * 0.1f;
                p.Scale = new(1f);
                p.NumberOfInnerReplicas = 6;
                p.ReplicaScalingFactor = 1.6f;
            });

            Particle.Create<SolarExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = Color.White.WithAlpha(127);
                p.Scale = new(0.2f);
                p.ScaleVelocity = new(0.12f);
                p.FrameSpeed = 3;
            });
        }

        public override Color? GetAlpha(Color lightColor) => Color.White.WithAlpha(127);
    }
}
