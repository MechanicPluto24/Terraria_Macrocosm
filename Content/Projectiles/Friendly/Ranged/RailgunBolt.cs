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
using Terraria.Audio;
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

    private bool killed;
    private int stickTime;

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.timeLeft = 270;
        Projectile.friendly = true;

        Projectile.penetrate = 5;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;

        Projectile.extraUpdates = 2;
        Projectile.tileCollide = false;
        stickTime = 60;
    }

    public override bool ShouldUpdatePosition() => true;


    public override void AI()
    {
        if (!killed)
        {
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        else
        {
            Projectile.Opacity = (Projectile.timeLeft / (float)stickTime);
            //Projectile.velocity *= 0.2f;
            float wiggle = MathHelper.Lerp(0.06f, 0f, 1f - Projectile.timeLeft / (float)stickTime);
            Projectile.rotation += (float)Math.Sin(Projectile.timeLeft) * wiggle;
        }

        if (!killed && !Collision.IsClearSpotTest(Projectile.Center, 16f, Projectile.width / 2, Projectile.height / 2, checkCardinals: true, checkSlopes: true))
    {
            Projectile.velocity = Vector2.Zero;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.friendly = false;
            Projectile.damage = 0;
            Projectile.extraUpdates = 0;
            Projectile.velocity = default;
            Projectile.timeLeft = stickTime;
            killed = true;
    }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => false;

    private void FalseKill()
    {
    }

    public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;
    public override bool PreDraw(ref Color lightColor)
    {
        if (!killed)
        {
            int trailCount = 16;
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
