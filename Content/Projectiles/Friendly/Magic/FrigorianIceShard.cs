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
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
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

            NPC closestNPC = Utility.GetClosestNPC(Projectile.Center, 9000f);
            if (closestNPC is not null)
            {
                Vector2 vel = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity += (vel * 0.9f);
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
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                float trailFactor = (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Color color = Projectile.GetAlpha(lightColor) * trailFactor;
                SpriteEffects effect = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Main.EntitySpriteDraw(tex, drawPos, tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame), color * 0.6f, Projectile.oldRot[i], Projectile.Size / 2, Projectile.scale, effect, 0f);
            }
            return true;
        }
    }
}
