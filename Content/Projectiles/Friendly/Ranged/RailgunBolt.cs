using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class RailgunBolt : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileSets.HitsTiles[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.timeLeft = 270;
        Projectile.friendly = true;

        Projectile.penetrate = 5;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;

        Projectile.extraUpdates = 1;
    }

    public override bool ShouldUpdatePosition() => true;

    private bool killed;
    public override void AI()
    {
        if (!killed)
        {
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        else
        {
            Projectile.Opacity -= 0.025f;
            Projectile.velocity *= 0.2f;
            float wiggle = MathHelper.Lerp(0.06f, 0f, 1f - Projectile.timeLeft / 60f);
            Projectile.rotation += (float)Math.Sin(Projectile.timeLeft) * wiggle;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!killed)
            FalseKill();

        return false;
    }

    private void FalseKill()
    {
        Projectile.friendly = false;
        Projectile.timeLeft = 60;
        Projectile.damage = 0;
        Projectile.extraUpdates = 0;
        Projectile.tileCollide = false;
        Projectile.velocity = Projectile.oldVelocity;
        killed = true;
    }

    public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;
    public override bool PreDraw(ref Color lightColor)
    {
        if (!killed)
        {
            int trailCount = 12;
            float distanceMult = 0.4f;
            for (int n = 0; n < trailCount; n++)
            {
                Vector2 trailPosition = Projectile.Center - new Vector2(10, 0).RotatedBy(Projectile.velocity.ToRotation()) * n * distanceMult;
                Color glowColor = (GetAlpha(lightColor) ?? default).WithAlpha(127) * (((float)trailCount - n) / trailCount) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, trailPosition - Main.screenPosition, null, glowColor, Projectile.rotation, TextureAssets.Projectile[Type].Size() / 2f, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
            }
        }

        return true;
    }
}
