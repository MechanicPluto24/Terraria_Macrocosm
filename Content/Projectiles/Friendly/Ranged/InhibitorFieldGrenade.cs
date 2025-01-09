using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class InhibitorFieldGrenade : ModProjectile
    {
        public int PlasmaBallCount
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public int BlastRadius
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            ProjectileID.Sets.Explosive[Type] = true;
            ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.timeLeft = 180;

            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.aiStyle = -1;

            PlasmaBallCount = 850;
            BlastRadius = 200;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            Lighting.AddLight(Projectile.Center, 0.407f, 1f, 1f);

            if (Projectile.timeLeft == 2)
                SpawnParticles();

            if (Projectile.timeLeft > 3)
            {
                float gravity = 0.6f * (0.5f + 0.5f * MacrocosmSubworld.GetGravityMultiplier());

                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Projectile.velocity.X is > -0.01f and < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.velocity.Y += gravity;
                Projectile.rotation += Projectile.velocity.X * 0.05f;

                if (Main.rand.NextBool(2))
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.InhibitorFieldDust>(), Scale: Main.rand.NextFloat(0.8f, 1.2f));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.4f;

            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
                Projectile.velocity.Y = oldVelocity.Y * -0.4f;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.timeLeft > 3)
                Projectile.timeLeft = 3;

            target.AddBuff(BuffID.Slow, 95);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.timeLeft > 3)
                Projectile.timeLeft = 3;

            target.AddBuff(BuffID.Slow, 95);
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize(BlastRadius, BlastRadius);
            Projectile.knockBack = 4f;
        }

        public void SpawnParticles()
        {
            Vector2 netOffset = Projectile.owner != Main.myPlayer ? new Vector2(BlastRadius / 2) : Vector2.Zero;

            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / PlasmaBallCount)
            {
                float speed = Utility.QuadraticEaseOut(Main.rand.NextFloat()) * 14f;
                float theta = i + Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) * 0.5f;
                Vector2 velocity = Utility.PolarVector(speed, theta);
                float factor = speed / 14f;

                var p = Particle.Create<InhibitorFieldParticle>(p =>
                {
                    p.Position = Projectile.Center + netOffset;
                    p.Velocity = velocity;
                    p.Scale = new Vector2(Main.rand.NextFloat(0.9f, 1.2f)) * (0.2f + 0.8f * factor);
                    p.FadeInNormalizedTime = 0f;
                    p.FadeOutNormalizedTime = Main.rand.NextFloat(0.2f, 0.8f);
                    p.TimeToLive = 205;
                    p.ScaleVelocity = new Vector2(-0.004f);
                });
            }

            /*
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / (PlasmaBallCount * 0.2f))
            {
                float speed = Utility.QuadraticEaseOut(Main.rand.NextFloat()) * 14f;
                float theta = i + Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) * 0.5f;
                Vector2 velocity = Utility.PolarVector(speed, theta);
                float factor = speed / 14f;
                Particle.Create<LightningParticle>((p) =>
                {
                    p.Position = Projectile.Center + netOffset;
                    p.Velocity = velocity * 0.9f;
                    p.Acceleration = velocity * 0.0008f;
                    p.Scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)) * factor;
                    p.TimeToLive = 205;
                    p.FadeOutNormalizedTime = 0.5f;
                    p.FrameSpeed = 6;
                    p.Color = new Color(104, 255, 255).WithAlpha((byte)Main.rand.Next(0, 64));
                    p.OutlineColor = new Color(104, 255, 255) * 0.2f;
                    p.ScaleVelocity = new Vector2(-0.006f);
                });
            }
            */

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = new(0.01f);
                p.ScaleVelocity = new(0.4f);
                p.Color = new Color(104, 255, 255, 255);
                p.TimeToLive = 4;
            });
        }
    }
}
