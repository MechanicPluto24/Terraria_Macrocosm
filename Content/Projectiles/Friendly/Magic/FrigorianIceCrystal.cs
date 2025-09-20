using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class FrigorianIceCrystal : ModProjectile
{
    /*
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }
    */

    public override void SetDefaults()
    {
        Projectile.scale = 1f;
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 500;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        Projectile.rotation += Projectile.velocity.X * 0.1f;
        Projectile.velocity.Y += 0.5f * (0.3f + 0.7f * MacrocosmSubworld.GetGravityMultiplier(Projectile.Center));

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

        for (int i = 0; i < (int)(oldVelocity.Y * 0.5f); i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.oldVelocity, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());
            dust.velocity.X = Main.rand.Next(-30, 31) * 0.04f;
            dust.velocity.Y = Main.rand.Next(-30, 30) * 0.04f;
            dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.001f;
            dust.noGravity = true;
        }

        return false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 3;
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition;
            float trailFactor = (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
            Color color = (Projectile.GetAlpha(lightColor) * trailFactor * 0.6f).WithAlpha((byte)(64 * trailFactor));
            SpriteEffects effect = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, drawPos, null, color * 0.6f, Projectile.oldRot[i], texture.Size() / 2, Projectile.scale * trailFactor, effect, 0f);
        }

        Main.EntitySpriteDraw(texture, Projectile.position - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);

        return false;
    }
}
