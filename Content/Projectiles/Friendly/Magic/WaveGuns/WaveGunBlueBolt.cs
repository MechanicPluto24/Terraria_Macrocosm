using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic.WaveGuns
{
    public class WaveGunBlueBolt : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public virtual int MaxPenetrate => 1;
        public virtual bool UseLocalImmunity => false;
        public virtual int LocalImmunityCooldown => 0;

        public virtual Color BeamColor => new(75, 75, 255, 0);
        public virtual Vector3 LightColor => new(0f, 0f, 1f);

        public virtual int DustCount => 60;
        public virtual int ParticleLightningCount => 12;
        public virtual float ParticleLightningSpread => 2f;
        public virtual float ParticleFlashScale => 0f;
        public virtual int TrailLength => 75;

        public int AI_Timer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        protected float spawnSpeed;
        protected LightningTrail trail;
        protected bool spawned;
        protected bool skipTrailUpdate;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Type] = -1; // custom trail updates
        }


        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600 * 5;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            if (!spawned)
            {
                Projectile.penetrate = MaxPenetrate;
                if (UseLocalImmunity)
                {
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.localNPCHitCooldown = LocalImmunityCooldown;
                }

                trail = new LightningTrail(BeamColor * 0.8f, 60f);
                spawnSpeed = Projectile.velocity.Length();
                spawned = true;
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians((int)(Math.Cos(AI_Timer / 10) / 10)));

            AI_Timer++;
            if (AI_Timer == 10)
                Projectile.tileCollide = true;

            if (Projectile.numUpdates % 2 == 0)
                Projectile.UpdateTrail(smoothAmount: skipTrailUpdate ? 0.01f : 0.65f);

            Lighting.AddLight(Projectile.Center, LightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FalseKill();
            return false;
        }

        protected void FalseKill()
        {
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = TrailLength;
            Projectile.velocity = default;
            Projectile.damage = 0;
            ParticleEffects();
        }

        public override void OnKill(int timeLeft) { }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => ParticleEffects();

        public virtual void ParticleEffects(float scale = 1f)
        {
            int dustCount = (int)(DustCount * scale);
            int lightningCount = (int)(ParticleLightningCount * scale);

            for (int i = 0; i < dustCount; i++)
            {
                Vector2 velocity = new Vector2(ParticleLightningSpread).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.1f, 0.6f));
                dust.noGravity = false;
                dust.color = BeamColor;
                dust.alpha = Main.rand.Next(60);
            }

            for (int i = 0; i < lightningCount; i++)
            {
                Particle.Create<LightningParticle>((p) =>
                {
                    p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                    p.Velocity = Main.rand.NextVector2Circular(8, 8);
                    p.Scale = new Vector2(Main.rand.NextFloat(0.1f, 1f));
                    p.FadeOutNormalizedTime = 0.5f;
                    p.Color = BeamColor.WithAlpha((byte)Main.rand.Next(0, 64));
                    p.OutlineColor = BeamColor * 0.2f;
                    p.ScaleVelocity = new(0.01f);
                });
            }

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = new(ParticleFlashScale * scale);
                p.ScaleVelocity = new(0.12f);
                p.Color = BeamColor.WithOpacity(scale);
            });
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            for (int n = 0; n < 4; n++)
            {
                var positions = (Vector2[])Projectile.oldPos.Clone();
                for (int i = 1; i < positions.Length; i++)
                {
                    if (positions[i] == default)
                        continue;

                    positions[i] += Main.rand.NextVector2Unit() * Main.rand.NextFloat((Math.Abs(trail.Saturation) + n * 2)  * Utility.InverseLerp(0, 120, AI_Timer, clamped: true));
                }

                int trailLength = TrailLength;
                if (AI_Timer < 8)
                    trailLength = 2;

                trail?.Draw(positions[..trailLength], Projectile.oldRot[..trailLength], Projectile.Size / 2f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            //Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + $"Trace{Main.rand.Next(2, 5).ToString()}").Value;
            //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale * 0.25f, SpriteEffects.None, 0f);

            return false;
        }
    }

}
