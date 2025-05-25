using Macrocosm.Common.CrossMod;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Debuffs.Radiation;

namespace Macrocosm.Content.Projectiles.Friendly.Misc
{
   public class AtomicWaveProjectile : ModProjectile
    {
        public override string Texture => "Macrocosm/Assets/Textures/LowRes/Circle3";
     
        protected bool spawned;

        public override void SetStaticDefaults()
        {
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Poison);
        }
        public override void SetDefaults()
        {
            Projectile.width = 512;
            Projectile.height = 512;
            Projectile.scale=0.1f;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 999;
            Projectile.Opacity=0f;
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                int DustCount = Main.rand.Next(450, 480);
                for (int i = 0; i < DustCount; i++)
                {
                    int dist = 80;
                    Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                    float distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                    float radians = (Projectile.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
                    Vector2 velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);
                    Particle.Create<DustParticle>((p =>
                    {
                        p.DustType = ModContent.DustType<IrradiatedDust>();
                        p.Position = dustPosition;
                        p.Velocity = velocity;
                        p.Acceleration = velocity * 80f;
                        p.Scale = new(Main.rand.NextFloat(1.8f, 2f));
                        p.NoGravity = true;
                        p.NormalUpdate = true;
                    }));


                    dist = 40;
                    dustPosition = Projectile.Center + Main.rand.NextVector2Circular(dist, dist);
                    distFactor = (Vector2.DistanceSquared(Projectile.Center, dustPosition) / (dist * dist));
                    radians = (Projectile.Center - dustPosition).ToRotation() - MathHelper.PiOver2;
                    velocity = new Vector2(-0.02f * distFactor, 0).RotatedBy(radians);

                    Particle.Create<PortalSwirl>((p =>
                    {
                        p.Position = dustPosition;
                        p.Scale = new(Main.rand.NextFloat(0.2f, 0.3f));
                        p.Velocity = velocity;
                        p.Acceleration = velocity * 40f;
                        p.TimeToLive = 30;
                        p.FadeInNormalizedTime = 0f;
                        p.FadeOutNormalizedTime = 0.9f;
                        p.Color = new Color(133, 255, 0);
                    }));
                }

                Particle.Create<TintableFlash>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Scale = new(0f);
                    p.ScaleVelocity = new(0.2f);
                    p.Color = new Color(133, 255, 0);
                });
            }
            
            Lighting.AddLight(Projectile.Center, new Color(201, 255, 0).ToVector3() * Projectile.Opacity);

            if (Projectile.timeLeft < 190)
                Projectile.friendly=false;

            if (Projectile.timeLeft > 190)
                Projectile.Opacity+=0.1f;
            else
                Projectile.Opacity*=0.9f;

            Projectile.scale = 0.1f + 1f * Projectile.Opacity;
            Projectile.width = (int)(512 * Projectile.scale);
            Projectile.height = (int)(512 * Projectile.scale);
            Projectile.Center=Main.player[Projectile.owner].Center;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(),240);
        }
        public override Color? GetAlpha(Color lightColor)
            => new Color(133, 255, 0) * Projectile.Opacity;
        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
       
            Vector2 origin = tex.Size() / 2f;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, new Color(133, 255, 0) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }

    }
}