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
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class CrudePhaserBolt : ModProjectile
{
    public override string Texture => Macrocosm.EmptyTexPath;

    private static readonly Color BeamColor = new(70, 255, 125, 0);

    private CrudePhaserTrail trail;
    private bool spawned;
    private bool emittedEffects;
    private bool falseKilled;
    private SpriteBatchState state;

    private bool PhasingShot => Projectile.ai[0] == 1f;
    private int TrailLength => PhasingShot ? 88 : 36;
    private int FlyTime => PhasingShot ? 420 : 285;
    private int AI_Timer
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 88;
        ProjectileID.Sets.TrailingMode[Type] = -1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 3600;
        Projectile.extraUpdates = 5;
    }

    public override void AI()
    {
        if (!spawned)
        {
            Projectile.penetrate = PhasingShot ? -1 : 1;
            Projectile.scale = PhasingShot ? 1.2f : 0.4f;

            if (PhasingShot)
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
            }

            trail = new CrudePhaserTrail(BeamColor * 0.8f, 60f);
            spawned = true;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        if (Projectile.numUpdates % 2 == 0)
            Projectile.UpdateTrail(smoothAmount: 0.55f);

        if (!falseKilled && ++AI_Timer >= FlyTime)
            FalseKill(0.45f);

        Lighting.AddLight(Projectile.Center, new Vector3(0.15f, 0.75f, 0.25f));
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        FalseKill(0.8f);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (PhasingShot)
        {
            KillEffects(0.55f);
        }
        else
        {
            FalseKill(0.8f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        if (!falseKilled)
            KillEffects(0.7f);
    }

    private void FalseKill(float effectScale)
    {
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = TrailLength;
        Projectile.velocity = Vector2.Zero;
        Projectile.damage = 0;
        falseKilled = true;
        KillEffects(effectScale);
    }

    private void KillEffects(float scale)
    {
        if (Main.dedServ)
            return;

        if (emittedEffects)
            return;

        emittedEffects = true;

        float impactScale = scale * (PhasingShot ? 1f : 0.35f);
        int dustCount = (int)(34 * impactScale);
        int particleCount = (int)(14 * impactScale);

        for (int i = 0; i < dustCount; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(PhasingShot ? 4.5f : 1.8f, PhasingShot ? 4.5f : 1.8f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.15f, 0.55f));
            dust.noGravity = false;
            dust.color = BeamColor;
            dust.alpha = Main.rand.Next(80);
        }

        for (int i = 0; i < particleCount; i++)
        {
            Particle.Create<LightningParticle>((p) =>
            {
                p.Position = Projectile.Center;
                p.Velocity = Main.rand.NextVector2Circular(PhasingShot ? 7f : 2.5f, PhasingShot ? 7f : 2.5f);
                p.Scale = new Vector2(Main.rand.NextFloat(0.1f, PhasingShot ? 0.75f : 0.3f));
                p.FadeOutNormalizedTime = 0.5f;
                p.Color = BeamColor.WithAlpha((byte)Main.rand.Next(0, 64));
                p.OutlineColor = BeamColor * 0.2f;
                p.ScaleVelocity = new(0.01f);
            });
        }

        Particle.Create<TintableFlash>((p) =>
        {
            p.Position = Projectile.Center;
            p.Scale = new((PhasingShot ? 0.1f : 0.035f) * scale);
            p.ScaleVelocity = new(0.1f);
            p.Color = BeamColor.WithOpacity(scale);
        });
    }

    public override bool PreDraw(Player player, ref Color lightColor)
    {
        state.SaveState(Main.spriteBatch);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.Additive, state);

        DrawTrail();

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(BlendState.AlphaBlend, state);

        DrawBolt();

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(state);

        return false;
    }

    private void DrawTrail()
    {
        if (trail is null)
            return;

        int trailLength = Math.Min(TrailLength, Projectile.oldPos.Length);
        if (AI_Timer < 8)
            trailLength = 2;

        for (int n = 0; n < 4; n++)
        {
            var positions = (Vector2[])Projectile.oldPos.Clone();
            for (int i = 1; i < positions.Length; i++)
            {
                if (positions[i] == default)
                    continue;

                positions[i] += Main.rand.NextVector2Unit() * Main.rand.NextFloat((Math.Abs(trail.Saturation) + n * 2) * Utility.InverseLerp(0, 120, 120 - Projectile.timeLeft, clamped: true));
            }

            trail.Draw(positions[..trailLength], Projectile.oldRot[..trailLength], Projectile.Size / 2f);
        }
    }

    private void DrawBolt()
    {
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        float rotation = Projectile.rotation + MathHelper.PiOver2;
        float scale = Projectile.scale;
        Color glowColor = new Color(70, 255, 125, PhasingShot ? 105 : 70);
        Color coreColor = new Color(170, 255, 190, PhasingShot ? 130 : 90);

        Utility.DrawStar(drawPosition, 1, glowColor, new Vector2(0.46f, 1.9f) * scale, rotation, entity: true);
        Utility.DrawStar(drawPosition, 1, coreColor, new Vector2(0.2f, 1.1f) * scale, rotation, entity: true);
        Utility.DrawStar(drawPosition, 1, glowColor, new Vector2(0.28f, 0.525f) * scale, rotation + MathHelper.PiOver2, entity: true);
    }
}
