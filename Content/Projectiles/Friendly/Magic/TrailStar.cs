using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class TrailStar : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 18;
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }

    private float currentAmplitude; //Current amplitude of the sine wave
    private float frequency = 0.125f;  // The frequency of the sine wave movement
    private float elapsedTime; // Pretty self-explanatory
    private float amplitudeDecay = 0.9f; // Percentage of amplitude retained after each cycle
    private int cycleCount = 0; // Keep track of the cycles so we can dimish the amplitude on a per-cycle basis
    private TrailScepterTrail trail;
    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4;

        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.timeLeft = 600;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;

        Projectile.light = 0.7f;
        Projectile.penetrate = 5;
        trail = new();
    }

    public override void OnSpawn(IEntitySource source)
    {
        currentAmplitude = Main.rand.NextFloat(-20f, 20f);
    }

    public override void AI()
    {
        elapsedTime += 1f;
        float phase = frequency * elapsedTime;
        // Check for completed cycles (2π radians is one complete cycle)
        if (phase >= (cycleCount + 1) * MathHelper.TwoPi)
        {
            cycleCount++;
            currentAmplitude *= amplitudeDecay;
        }

        float sineOffset = currentAmplitude * (float)Math.Sin(phase);
        Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * sineOffset;
        Projectile.position += offset;

        Projectile.rotation = (Projectile.position - Projectile.oldPosition).ToRotation();

        for (int i = 0; i < 2; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GreenBrightDust>(), default, Scale: Main.rand.NextFloat(0.4f, 0.8f));
            d.noGravity = true;
        }

        var star = Particle.Create<ImbriumStar>(Projectile.Center + Main.rand.NextVector2Circular(8, 8) + Projectile.velocity * 0.5f, Vector2.Zero, scale: new(0.34f));
        star.Opacity = 0.8f;
    }

    private SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        state.SaveState(Main.spriteBatch);
        Texture2D tex = TextureAssets.Projectile[Type].Value;

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);

        trail?.Draw(Projectile, Projectile.Size / 2f);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);

        Rectangle sourceRect = tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);
        Vector2 origin = tex.Size() / 2f;

        for (int i = 0; i < 12; i++)
        {
            float opacityMultiplier = 0.05f * (i + 1);
            float scaleMultiplier = 1.2f - (0.1f * i);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, sourceRect, (new Color(0, 217, 102, 0)) * opacityMultiplier * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * scaleMultiplier, SpriteEffects.None, 0);
        }

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 10; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.Center, 16, 16, ModContent.DustType<GreenBrightDust>(), Scale: Main.rand.NextFloat(0.2f, 1.5f));
            d.noGravity = true;
        }

        for (int i = 0; i < 4; i++)
        {
            var star = Particle.Create<ImbriumStar>(new Vector2(Projectile.position.X, Projectile.position.Y) + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, (int)Projectile.Size.X, (int)Projectile.Size.Y)), Main.rand.NextVector2Circular(1f, 1f), scale: new(0.4f));
            star.Opacity = 0.8f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.penetrate--;
        if (Projectile.penetrate <= 0)
        {
            Projectile.Kill();
        }
    }
}
