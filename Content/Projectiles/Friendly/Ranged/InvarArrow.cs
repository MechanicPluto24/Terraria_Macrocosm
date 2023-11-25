using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class InvarArrow : ModProjectile, IBullet
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.timeLeft = 270;
            Projectile.light = 0f;
            Projectile.friendly = true;

            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(0f - Projectile.velocity.Y, 0f - Projectile.velocity.X) - 1.57f;
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            }

            //for(int i = 0; i < 2; i++)
            //{
            //	Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SeleniteBrightDust>(), 0, 0, Scale: Main.rand.NextFloat(0.8f, 1f));
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
                Dust.NewDustPerfect(Projectile.Center + Projectile.oldVelocity * 0.5f, ModContent.DustType<InvarBits>(), Projectile.oldVelocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.4f), Scale: 3f);
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            state = Main.spriteBatch.SaveState();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            int trailCount = 80;
            float distanceMult = 0.25f;
            for (int n = 1; n < trailCount; n++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * distanceMult;
                Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, Color.White * (0.55f - (float)n / trailCount), Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
        }

        public override Color? GetAlpha(Color lightColor)
            => new Color(255, 255, 255, 200);
    }
}
