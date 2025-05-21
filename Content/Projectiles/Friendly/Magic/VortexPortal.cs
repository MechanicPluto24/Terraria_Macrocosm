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

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
   public class VortexPortal : ModProjectile
    {
        public ref float AITimer => ref Projectile.ai[0];

        public bool Phase2
        {
            get => Projectile.ai[1] > 0f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public int TargetPlayer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public int SpawnPeriod => 12;

        public const int PortalTimerMax = (int)(4f * 60 + 1.5f * 60 + 24);

        protected int defWidth;
        protected int defHeight;

        protected bool spawned;

        public override void SetStaticDefaults()
        {
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Thunder);
            Redemption.AddElementToProjectile(Type, Redemption.ElementID.Celestial);
        }
        public override void SetDefaults()
        {
            defWidth = defHeight = Projectile.width = Projectile.height = 52;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = PortalTimerMax;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.alpha = 255;
            Projectile.scale=2f;
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                AITimer = 255f;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item78, Projectile.Center);
            }

            Lighting.AddLight(Projectile.Center, new Color(30, 255, 105).ToVector3() * (1f - Projectile.alpha / 255f));

            Projectile.rotation -= MathHelper.ToRadians(7.4f);

            if (Projectile.timeLeft >= PortalTimerMax - 30)
                AITimer -= 2.83333325f*3f;
            else if (Projectile.timeLeft <= 30)
                AITimer += 2.83333325f*3f;
            else
            {
                AITimer = 0f;
            }

            for (int i = 0; i < 10; i++)
            {
                float progress = (1f - AITimer / 255f);
                Particle.Create<PortalSwirl>(p =>
                {
                    p.Position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * 1.6f * progress;
                    p.Velocity = Vector2.One * 18;
                    p.Scale = new((0.1f + Main.rand.NextFloat(0.1f)) * progress);
                    p.Color = new Color(92, 206, 130);
                    p.TargetCenter = Projectile.Center;
                });
            }

            Projectile.alpha = (int)MathHelper.Clamp((int)AITimer, 0f, 255f);

            Vector2 center = Projectile.Center;
            Projectile.scale = 0.05f + 0.95f * (1f - Projectile.alpha / 255f);
            Projectile.width = (int)(defWidth * Projectile.scale);
            Projectile.height = (int)(defHeight * Projectile.scale);
            Projectile.Center = center;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.Center+=(Projectile.Center-target.Center).SafeNormalize(Vector2.UnitX)*target.knockBackResist*30f;
        }
        public override Color? GetAlpha(Color lightColor)
            => Color.White * (1f - Projectile.alpha / 255f);

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Color color = Color.White;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 1f).WithOpacity(0.5f - 0.5f * Projectile.alpha / 255f), (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 1.4f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (color * 0.84f).WithOpacity(0.5f - 0.5f * Projectile.alpha / 255f), Projectile.rotation, texture.Size() / 2f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * 1f, (0f - Projectile.rotation) * 0.65f, texture.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Flare3").Value;
            float scale = Projectile.scale * Main.rand.NextFloat(0.9f, 1.1f);
            Main.EntitySpriteDraw(flare, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, new Color(30, 255, 105).WithOpacity(0.25f), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);

            // Strange
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}