using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class ImbriumJewelMeteor : ModProjectile
    {
		public override void SetStaticDefaults()
		{
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 12f)
                Projectile.velocity.Y = 12f;

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Main.rand.NextFloat() < 0.8f)
            {
                int dustIdx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 128, new Color(0, 255, 100), 1.8f);
                Main.dust[dustIdx].noGravity = true;
                Main.dust[dustIdx].velocity.X *= 2f;
                Main.dust[dustIdx].velocity.Y *= 1.5f;
            }


            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Type]; // 6 frames @ 4 ticks/frame
            }

            if (Projectile.alpha < 250)
                Projectile.alpha += 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;

            Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

            Vector2 origin = Projectile.Size / 2f + new Vector2(0, 16);

            SpriteBatchState state = Main.spriteBatch.SaveState();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition,
                sourceRect, new Color(255, 255, 255, Projectile.alpha), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Restore(state);
            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIdx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 128, new Color(255, 255, 255), 3f);
                Main.dust[dustIdx].noGravity = true;
                Main.dust[dustIdx].velocity.X *= 2f;
                Main.dust[dustIdx].velocity.Y *= 1.5f;

			}
		}
    }
}