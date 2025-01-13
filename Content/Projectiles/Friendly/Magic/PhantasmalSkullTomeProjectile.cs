using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class PhantasmalSkullTomeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        private static int spawnTimeLeft = 2 * 60;
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = spawnTimeLeft;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            CooldownSlot = 1;
        }

        private bool spawned;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
            => false;

        public enum ActionState
        {
            Move,
            Home
        }

        public ActionState AI_State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        public int AI_Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public Vector2 AI_Target
        {
            get => new (Projectile.ai[1], Projectile.ai[2]);
            set { Projectile.ai[1] = value.X; Projectile.ai[2] = value.Y; }
        }

        private float originalSpeed;

        public override void AI()
        {
            if (!spawned)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchRange = (-0.5f, 0.5f) }, Projectile.Center);

                Particle.Create<PhantasmalSkullSpawnEffect>((p) =>
                    {
                        p.Position = Projectile.Center + Projectile.velocity * 3;
                        p.Velocity = Vector2.Zero;
                    },
                    shouldSync: true
                );

                originalSpeed = Projectile.velocity.Length();

                if (Projectile.owner == Main.myPlayer)
                {
                    AI_Target = Main.MouseWorld + Main.rand.NextVector2Circular(50f, 150f);
                    Projectile.netUpdate = true;
                }

                Projectile.alpha = 255;
                spawned = true;
            }

            Lighting.AddLight(Projectile.Center, new Color(30, 255, 105).ToVector3() * Projectile.Opacity);

            AI_Timer++;

            if (AI_Timer < 25)
            {
                Projectile.Opacity += 0.1f;
            }
            else
            {
                Vector2 target = Vector2.Zero;
                float dist = Vector2.DistanceSquared(Projectile.Center, AI_Target);
                if (dist > 30 * 30)
                    target = AI_Target;

                if (target != Vector2.Zero)
                {
                    Vector2 aim = (target - Projectile.Center).SafeNormalize(default);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(default), aim, 0.1f) * originalSpeed;
                }

                Projectile.Opacity = Utility.CubicEaseOut((float)Projectile.timeLeft / spawnTimeLeft);
            }

            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle.Create<PhantasmalSkullHitEffect>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 2;
                p.Velocity = Vector2.Zero;
                p.Scale = new(Main.rand.NextFloat(0.1f, 0.25f));
            }, shouldSync: true
                );
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int length = Projectile.oldPos.Length;
            for (int i = 1; i < length; i++)
            {
                float progress = i / (float)length;

                float wave = MathF.Sin(((length - i) + AI_Timer)) * (2f + 12f * progress);
                Vector2 waveOffset = Math.Abs(Projectile.velocity.X) > Math.Abs(Projectile.velocity.Y) ? new Vector2(0, wave) : new Vector2(wave, 0);
                Vector2 drawPos = Projectile.oldPos[i];
                if (i > 0) drawPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.position, 0.3f);
                drawPos += Projectile.Size / 2f + waveOffset - Main.screenPosition;

                Color trailColor = new Color(127, 127, 127, 0) * Projectile.Opacity * Utility.QuadraticEaseIn(1f - progress) * 0.75f;
                float rotation = Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.oldRot[i] : Projectile.oldRot[i];
                float scale = Projectile.scale * 0.8f * (1f - progress);

                Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, drawPos, null, trailColor , rotation, Projectile.Size / 2f, (float)scale, effects, 0f);
            }

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, Color.White.WithOpacity(0.8f) * Projectile.Opacity, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, effects, 0f);
            return false;
        }
    }
}
