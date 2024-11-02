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
    public class WaveGunEnergyBolt : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 75;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public enum BeamVariant
        {
            Blue,
            Red,
            Purple
        }

        public BeamVariant BeamType
        {
            get => (BeamVariant)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        public int AI_Timer
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private Color color;
        private Vector3 lightColor;
        private WaveGunBeamTrail trail;
        private bool spawned;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            if (!spawned)
            {
                if (BeamType == BeamVariant.Blue)
                {
                    Projectile.penetrate = 1;
                    color = new Color(75, 75, 255, 0);
                    lightColor = new Vector3(0, 0, 1f);
                    trail = new WaveGunBeamTrail(new Color(75, 75, 255, 0) * 0.8f);
                }
                else if (BeamType == BeamVariant.Red)
                {
                    Projectile.penetrate = 1;
                    color = new Color(255, 75, 75, 0);
                    lightColor = new Vector3(1f, 0, 0f);
                    trail = new WaveGunBeamTrail(new Color(255, 75, 75, 0) * 0.8f);
                }
                else if (BeamType == BeamVariant.Purple)
                {
                    Projectile.penetrate = -1;
                    Projectile.usesLocalNPCImmunity = true;
                    Projectile.localNPCHitCooldown = -1;
                    color = new Color(255, 150, 255, 0);
                    lightColor = new Vector3(1f, 0, 1f);
                    trail = new WaveGunBeamTrail(new Color(255, 150, 255, 0) * 0.8f);
                }

                spawned = true;
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians((int)(Math.Cos(AI_Timer / 10) / 10)));

            AI_Timer++;

            Projectile.tileCollide = AI_Timer > 10;
            Lighting.AddLight(Projectile.Center, lightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < (BeamType is BeamVariant.Purple ? 120 : 60); i++)
            {
                Vector2 velocity = (BeamType is BeamVariant.Purple ? new Vector2(3f) : new Vector2(2f)).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                Dust dust = Dust.NewDustPerfect(Projectile.oldPosition + Projectile.Size / 2f + Projectile.oldVelocity, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.1f, 0.6f));
                dust.noGravity = false;
                dust.color = color;
                dust.alpha = Main.rand.Next(60);
            }

            for (int i = 0; i < (BeamType is BeamVariant.Purple ? 18 : 12); i++)
            {
                Particle.Create<LightningParticle>((p) =>
                {
                    p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                    p.Velocity = Main.rand.NextVector2Circular(8, 8);
                    p.Scale = new(Main.rand.NextFloat(0.1f, 1f));
                    p.FadeOutNormalizedTime = 0.5f;
                    p.Color = color.WithAlpha((byte)Main.rand.Next(0, 64));
                    p.OutlineColor = color * 0.2f;
                    p.ScaleVelocity = new(0.01f);
                });
            }

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = (BeamType is BeamVariant.Purple ? new(0.1f) : new(0f));
                p.ScaleVelocity = new(0.12f);
                p.Color = color.WithAlpha(255);
            });

        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + $"Trace{Main.rand.Next(2, 5).ToString()}").Value;
            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            var positions = (Vector2[])Projectile.oldPos.Clone();
            for (int i = 1; i < positions.Length; i++)
            {
                if (positions[i] == default)
                    continue;

                positions[i] += Main.rand.NextVector2Unit() * Main.rand.NextFloat(5f * Utility.InverseLerp(0, 120, AI_Timer, clamped: true));
            }

            int trailLength = BeamType is BeamVariant.Purple ? Projectile.oldPos.Length : Projectile.oldPos.Length - 10;

            if (AI_Timer < 8)
                trailLength = 2;

            trail?.Draw(positions[..trailLength], Projectile.oldRot[..trailLength], Projectile.Size / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale * 0.25f, SpriteEffects.None, 0f);

            return false;
        }
    }
}

