using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class ImbriumJewelMeteor : ModProjectile
{
    private ImbriumMeteorTrail trail;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 3;
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
        trail = new();
    }

    public override void AI()
    {
        Projectile.velocity.Y += 0.1f;
        if (Projectile.velocity.Y > 12f)
            Projectile.velocity.Y = 12f;

        if (Projectile.velocity != Vector2.Zero)
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        if (Main.rand.NextFloat() < 0.9f)
        {
            var star = Particle.Create<ImbriumStar>(Projectile.Center + Main.rand.NextVector2Circular(8, 8), Vector2.Zero, scale: new(0.34f));
            star.Opacity = 0.8f;
        }

        if (++Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Type]; // 6 frames @ 4 ticks/frame
        }

        if (Projectile.alpha < 250)
            Projectile.alpha += 10;
    }

    private SpriteBatchState state;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;

        Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

        Vector2 origin = Projectile.Size / 2f + new Vector2(0, 16);

        state.SaveState(Main.spriteBatch);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);

        trail?.Draw(Projectile, Projectile.Size / 2f);

        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition,
            sourceRect, new Color(255, 255, 255, Projectile.alpha), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);
        return false;
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox)
    {
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
        for (int i = 0; i < 10; i++)
        {
            var star = Particle.Create<ImbriumStar>(new Vector2(Projectile.position.X, Projectile.position.Y) + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, (int)Projectile.Size.X, (int)Projectile.Size.Y)), Main.rand.NextVector2Circular(1f, 1f), scale: new(0.8f));
            star.Opacity = 0.8f;
        }

        for (int j = 0; j < ProjectileID.Sets.TrailCacheLength[Type]; j++)
        {
            float progress = 1f - (j / (float)ProjectileID.Sets.TrailCacheLength[Type]);
            var star = Particle.Create<ImbriumStar>(Projectile.oldPos[j] + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, (int)Projectile.Size.X, (int)Projectile.Size.Y)), Main.rand.NextVector2Circular(0.1f, 0.1f), scale: new(0.4f * progress));
            star.Opacity = 0.8f * progress;
        }
    }
}