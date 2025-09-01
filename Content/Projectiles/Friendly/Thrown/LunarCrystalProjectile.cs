using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Thrown
{
    public class LunarCrystalProjectile : ModProjectile
    {
        public override string Texture => "Macrocosm/Content/Items/Consumables/Throwable/LunarCrystal";
        public int BounceCounter
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private int numBounces = 10;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 3600;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.oldVelocity = oldVelocity;

            Bounce(oldVelocity);

            if (BounceCounter > numBounces)
                Projectile.velocity.X *= 0.8f;

            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(0, 255, 180).ToVector3() * 2f);
            Projectile.velocity.Y += 1f * (0.1f + 0.9f * MacrocosmSubworld.GetGravityMultiplier(Projectile.Center));
            Projectile.oldVelocity = Projectile.velocity * -1f;
            if (BounceCounter < numBounces)
                Projectile.rotation += 0.05f * Projectile.velocity.Length() * Math.Sign(Projectile.velocity.X);
        }

        private void Bounce(Vector2 oldVelocity)
        {
            if (BounceCounter < numBounces)
            {
                for (int i = 0; i < (int)(oldVelocity.Y * 4f); i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LuminiteBrightDust>());
                    dust.velocity.X = Main.rand.Next(-70, 71) * 0.04f;
                    dust.velocity.Y = Main.rand.Next(-70, 70) * 0.04f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.02f;
                    dust.noGravity = true;
                }
            }

            BounceCounter++;
            float bounceFactor = 0.5f + 0.45f * BounceCounter / (float)numBounces;

            if (BounceCounter < numBounces)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = oldVelocity.X * -bounceFactor;

                if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
                    Projectile.velocity.Y = oldVelocity.Y * -bounceFactor;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int numFrames = Main.projFrames[Type];
            Rectangle sourceRect = texture.Frame(1, numFrames, frameY: Projectile.frame);
            Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, sourceRect, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            return false;
        }
    }
}
