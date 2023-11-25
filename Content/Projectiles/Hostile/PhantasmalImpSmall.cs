using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class PhantasmalImpSmall : ModProjectile
    {
        public Player TargetPlayer => Main.player[(int)Projectile.ai[2]];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        private static int spawnTimeLeft = 3 * 60;
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 54;
            Projectile.hostile = true;
            Projectile.timeLeft = spawnTimeLeft;
            Projectile.alpha = 0;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
            => false;

        //bool spawned = false;
        public override void AI()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            if (!TargetPlayer.active)
                Projectile.Kill();

            Vector2 direction = TargetPlayer.Center - Projectile.Center;
            direction.Normalize();

            // Apply a slight random deviation to the direction
            float deviation = Main.rand.NextFloat(-0.1f, 0.1f);
            direction = direction.RotatedBy(deviation);

            // Apply a slight random deviation to the direction
            Vector2 adjustedDirection = Vector2.Lerp(Projectile.velocity, direction, 0.2f);
            adjustedDirection.Normalize();

            Projectile.velocity = adjustedDirection * Projectile.velocity.Length();

            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation();

            if (Projectile.timeLeft < (int)(0.33f * spawnTimeLeft) && Projectile.alpha < 255)
                Projectile.alpha += 6;

            if (Projectile.alpha >= 255)
                Projectile.active = false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White.WithOpacity(1f);

        private SpriteBatchState state;

        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            for (int i = 1; i < length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;

                Color trailColor = Color.White * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, drawPos, null, trailColor, Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.oldRot[i] : Projectile.oldRot[i], Projectile.Size / 2f, Projectile.scale, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.NonPremultiplied, state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, Color.White.WithOpacity(0.7f * (1f - Projectile.alpha / 255f)), Projectile.rotation, Projectile.Size / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}
