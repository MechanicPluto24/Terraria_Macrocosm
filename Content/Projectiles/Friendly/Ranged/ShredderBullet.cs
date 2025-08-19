using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class ShredderBullet : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 2;

        ProjectileSets.HitsTiles[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.alpha = 255;
        Projectile.scale = 1.2f;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.extraUpdates = 3;
        Projectile.timeLeft = 270;
    }

    public override bool PreAI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Projectile.alpha > 0)
            Projectile.alpha -= 15;

        if (Projectile.alpha < 0)
            Projectile.alpha = 0;

        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawMagicPixelTrail(new Vector2(0, 0), 4f, 0f, new Color(0, 178, 115), new Color(0, 178, 115, 0));
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Bleeding, 60);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.Bleeding, 60);
    }

    //public override Color? GetAlpha(Color lightColor) => Color.White;
}
