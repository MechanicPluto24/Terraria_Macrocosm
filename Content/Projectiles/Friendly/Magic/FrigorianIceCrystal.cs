using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class FrigorianIceCrystal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.scale = 1f;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Projectile.velocity.Y += 0.5f * MacrocosmSubworld.CurrentGravityMultiplier;

            if (Main.rand.NextBool(12))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());

            if (Projectile.timeLeft < 50 && Projectile.alpha < 255)
                Projectile.alpha += 5;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.6f;

            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
                Projectile.velocity.Y = oldVelocity.Y * -0.6f;

            for (int i = 0; i < (int)(oldVelocity.Y * 3f); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());
                dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
                dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
                dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
                dust.noGravity = true;
            }

            return false;
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
