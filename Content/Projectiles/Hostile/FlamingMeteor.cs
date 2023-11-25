using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    //Had to salvage it from an extracted DLL, so no comments.  Oops.  -- absoluteAquarian
    public class FlamingMeteor : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 60;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;

            Projectile.SetTrail<FlamingMeteorTrail>();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 360, true);
            target.AddBuff(BuffID.Burning, 90, true);
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.068f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;


            float dustTrailCount = MathHelper.Lerp(0f, 2f, Utils.GetLerpValue(0f, 16f, Math.Abs(Projectile.velocity.Y))) +
                                  MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(0f, 9.25f, Math.Abs(Projectile.velocity.X)));

            for (int i = 0; i < dustTrailCount; i++)
            {
                if (Math.Abs(Projectile.velocity.Y) < 0.5f && Math.Abs(Projectile.velocity.X) < 0.5f)
                    break;

                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 128, default, 3f);
                dust.noGravity = true;
                dust.velocity.X *= 2f;
                dust.velocity.Y *= 1.5f;
                dust.scale = MathHelper.Lerp(0.5f, 2.5f, Utils.GetLerpValue(28, 0, Vector2.Distance(dust.position, Projectile.Center)));

                if (dust.dustIndex % 2 == 0)
                    dust.color = new Color(0, 255, 100);
            }


            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Type]; // 6 frames @ 4 ticks/frame
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
            Vector2 origin = Projectile.Size / 2f + new Vector2(6, 32);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

            Projectile.GetTrail().Draw(Projectile.Size / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition,
                sourceRect, Color.White.WithOpacity(0f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
        }
    }
}
