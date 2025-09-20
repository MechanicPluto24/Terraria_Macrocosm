using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee;

public class ProcellarumLightBolt : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 45;
        Projectile.height = 45;
        DrawOffsetX = -30;

        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 4;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        if (Projectile.ai[0] != 0)
            HomeIn(Projectile.ai[0] - 1, Projectile.ai[1]);
        else
            Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public void HomeIn(float targetIndex, float firedIndex)
    {
        NPC target = Main.npc[(int)targetIndex];
        float TargetRotation = (target.Center - Projectile.Center).ToRotation();
        float currentRotation = Projectile.velocity.ToRotation();
        if (TargetRotation - currentRotation <= MathHelper.PiOver4 && TargetRotation - currentRotation >= -MathHelper.PiOver4)
        {
            Projectile.rotation = 0.5f * (TargetRotation + currentRotation);
            Projectile.velocity = Utility.PolarVector(40 + firedIndex * 4, Projectile.rotation);
        }
        else
        {
            Projectile.rotation = currentRotation;
        }
    }

    public override void OnKill(int timeLeft)
    {
        float strength = 0.3f;
        Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ProcellarumExplosion>(), Projectile.damage, 12f, Main.myPlayer, ai0: strength, ai1: -1f);
    }

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
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawPos, null, trailColor, Projectile.oldRot[i], Projectile.Size / 2f, Projectile.scale, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, GetAlpha(lightColor) ?? Color.White, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        return false;
    }

    public override Color? GetAlpha(Color lightColor) => Color.White;
}
