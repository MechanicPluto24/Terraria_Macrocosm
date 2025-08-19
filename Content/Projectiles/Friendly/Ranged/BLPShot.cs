using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class BLPShot : ModProjectile
{
    private bool spawned;
    private Color color = default;

    public override string Texture => Macrocosm.FancyTexturesPath + "Trace2";

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
        Projectile.scale = 1f;
        Projectile.alpha = 165;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        color = new Color(144, 255, 255, 255);
    }

    public override void AI()
    {
        if (!spawned)
        {
            spawned = true;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        if (Projectile.alpha > 0)
            Projectile.alpha -= 50;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override void OnKill(int timeLeft)
    {
        int dustCount = 22;
        int lightningCount = 10;

        for (int i = 0; i < dustCount; i++)
        {
            Vector2 velocity = new Vector2(2f, 0).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
            Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.1f, 0.6f));
            dust.noGravity = false;
            dust.color = color.WithAlpha(255);
            dust.alpha = 64;
        }

        for (int i = 0; i < lightningCount; i++)
        {
            Particle.Create<LightningParticle>((p) =>
            {
                p.Position = Projectile.Center;
                p.Velocity = Main.rand.NextVector2Circular(4, 4);
                p.Scale = new Vector2(Main.rand.NextFloat(0.1f, 0.4f));
                p.FadeOutNormalizedTime = 0.5f;
                p.Color = color.WithAlpha((byte)Main.rand.Next(0, 64));
                p.OutlineColor = color * 0.2f;
                p.ScaleVelocity = new(0.01f);
            });
        }

        Particle.Create<TintableFlash>((p) =>
        {
            p.Position = Projectile.Center ;
            p.Scale = new(0.1f);
            p.ScaleVelocity = new(0.01f);
            p.Color = color.WithOpacity(0.5f);
        });
    }

    public override Color? GetAlpha(Color lightColor) => color * Projectile.Opacity;

    private SpriteBatchState state;
    public override bool PreDraw(ref Color lightColor)
    {
        state.SaveState(Main.spriteBatch);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);

        var fireball = TextureAssets.Projectile[Type];
        Main.EntitySpriteDraw(fireball.Value, Projectile.position - Main.screenPosition + new Vector2(-12, 0).RotatedBy(Projectile.velocity.ToRotation()), null, color, Projectile.rotation + MathHelper.PiOver2, fireball.Size() / 2, Projectile.scale * 0.2f, SpriteEffects.None, 0);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);

        return false;
    }
}

