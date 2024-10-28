using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class FrigorianIceShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 500;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.alpha > 0)
                Projectile.alpha -= 25;

            Projectile.velocity.Y += 0.1f;

            NPC closestNPC = Utility.GetClosestNPC(Projectile.Center, 9000f);
            if (closestNPC is not null)
            {
                Vector2 vel = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity += (vel * 0.4f);
                Projectile.velocity = (Projectile.velocity).SafeNormalize(Vector2.UnitX);
                Projectile.velocity *= 17f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());
                dust.velocity.X = Main.rand.Next(-70, 71) * 0.02f;
                dust.velocity.Y = Main.rand.Next(-70, 70) * 0.02f;
                dust.scale *= 1f + Main.rand.Next(-15, 16) * 0.01f;
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition;
                float trailFactor = (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Color trailColor = (i % 2 == 0 ? new Color(56, 188, 173, 0) : new Color(93, 81, 164, 0)) * (trailFactor * 0.45f * (1f - Projectile.alpha / 255f));
                Main.EntitySpriteDraw(glow, drawPos, null, trailColor, Projectile.oldRot[i] + MathHelper.PiOver2, glow.Size() / 2f, Projectile.scale * trailFactor, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            return false;
        }
    }
}
