using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class InvarArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.timeLeft = 270;
            Projectile.friendly = true;

            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(0f - Projectile.velocity.Y, 0f - Projectile.velocity.X) - MathHelper.PiOver2;
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
            }

            //for(int i = 0; i < 2; i++)
            //{
            //	Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteBrightDust>(), 0, 0, Scale: Main.rand.NextFloat(0.8f, 1f));
            //	dust.noGravity = true;
            //}

            Lighting.AddLight(Projectile.Center, new Color(255, 255, 255).ToVector3() * 0.6f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.65f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.damage = (int)(Projectile.damage * 0.65f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
                Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity * 0.5f, ModContent.DustType<InvarBits>(), new Vector2(10, 0).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(), Scale: 3f);

            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity * 0.5f;
                p.Scale = new(0.5f);
                p.ScaleVelocity = new(0.01f);
                p.Color = new Color(157, 125, 36).WithOpacity(0.5f);
            });
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            state = Main.spriteBatch.SaveState();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            int trailCount = 60;
            float distanceMult = 0.4f;
            for (int n = 5; n < trailCount; n++)
            {
                Vector2 trailPosition = Projectile.Center - new Vector2(10, 0).RotatedBy(Projectile.velocity.ToRotation()) * n * distanceMult + Main.rand.NextVector2Circular(5, 5);
                Color glowColor = new Color(165, 146, 90, 255) * (((float)trailCount - n) / trailCount) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.SharpTears].Value, trailPosition - Main.screenPosition, null, glowColor, Projectile.rotation, TextureAssets.Extra[ExtrasID.SharpTears].Size() / 2f, Projectile.scale * 1.2f, SpriteEffects.None, 0f);

            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return true;
        }

        public override void PostDraw(Color lightColor)
        { 
        }

        public override Color? GetAlpha(Color lightColor)
            => new Color(255, 255, 255, 127);
    }
}
