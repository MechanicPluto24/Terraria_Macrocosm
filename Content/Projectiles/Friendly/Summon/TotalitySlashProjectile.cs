using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Global.Projectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Weapons;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon;

public class TotalitySlashProjectile : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 100;
        ProjectileID.Sets.TrailingMode[Type] = 3;
    }

    public ref float AI_Timer => ref Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.scale = 1f + (float)Main.rand.Next(30) * 0.01f;
        Projectile.extraUpdates = 2;
        Projectile.timeLeft = 480;

        oldPosLerped = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
        oldRotLerped = new float[ProjectileID.Sets.TrailCacheLength[Type]];
    }

    private bool spawned;
    private Color color;

    private Vector2[] oldPosLerped;
    private float[] oldRotLerped;

    private int slashDir;

    public override void AI()
    {
        if (!spawned)
        {
            color = Main.rand.Next(3) switch
            {
                0 => new(44, 210, 91),
                1 => new(201, 125, 205),
                _ => new(114, 111, 207),
            };
            spawned = true;

            slashDir = Main.player[Projectile.owner].direction;
        }


        Player player = Main.player[Projectile.owner];
        AI_Timer++;

        if (AI_Timer > 100)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / 128 * slashDir);
            Projectile.velocity *= 0.995f;

            for (int i = 0; i < oldPosLerped.Length; i++)
            {
                if (Projectile.oldPos[i] == default)
                    Projectile.oldPos[i] = Projectile.position;

                oldRotLerped[i] = Projectile.oldRot[i];
                oldPosLerped[i] = Vector2.Lerp(oldPosLerped[i], Projectile.oldPos[i], 0.2f);
                oldPosLerped[i] += Projectile.velocity * 0.05f;
            }
        }
        else
        {
            for (int i = 0; i < oldPosLerped.Length; i++)
            {
                if (Projectile.oldPos[i] == default)
                    Projectile.oldPos[i] = Projectile.position;

                oldPosLerped[i] = Projectile.oldPos[i];
                oldRotLerped[i] = Projectile.oldRot[i];
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / 80 * slashDir);
            Projectile.position += player.velocity * 0.2f;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.Opacity = Projectile.timeLeft / 480f;

        for (int i = 0; i < oldPosLerped.Length; i++)
        {
            if (oldPosLerped[i] == default)
                continue;

            if (Main.rand.NextBool(100))
                Particle.Create<DarkCelestialStar>((p) =>
                {
                    p.Position = oldPosLerped[i] + Projectile.Size / 2f;
                    p.Velocity = Vector2.Zero;
                    p.Scale = new Vector2(1f, 1f) * Main.rand.NextFloat(0.7f) * Projectile.Opacity;
                    p.TimeToLive = 15;
                    p.Color = color;
                });

            Lighting.AddLight(oldPosLerped[i] + Projectile.Size / 2f, color.ToVector3() * 0.5f * Projectile.Opacity);
        }

        if (Projectile.soundDelay == 0)
        {
            Projectile.soundDelay = -1;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        // minions will attack the npcs hit with this whip 
        Player player = Main.player[Projectile.owner];
        player.MinionAttackTargetNPC = target.whoAmI;

        target.AddBuff(ModContent.BuffType<TotalityTag>(), 2 * 60);

        float rotation = (target.Center - player.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8);
        for (float f = 0f; f < 1f; f += 1f / (6f * Projectile.Opacity))
        {
            rotation = MathHelper.TwoPi * f + Main.rand.NextFloat() * MathHelper.TwoPi + Main.rand.NextFloatDirection() * 0.25f;

            Particle.Create<LightningParticle>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = rotation.ToRotationVector2() * (Main.rand.NextFloat() * 6f) * new Vector2(0.6f, 1f);
                p.Rotation = rotation;
                p.Color = (color * 0.3f).WithAlpha(127);
                p.OutlineColor = (color * 0.9f).WithAlpha(127);
                p.Scale = new(Main.rand.NextFloat(0.8f, 1.4f));
                p.ScaleVelocity = new Vector2(-0.05f);
                p.FadeInNormalizedTime = 0.01f;
                p.FadeOutNormalizedTime = 0.7f;
            });
        }

        for (float f = 0f; f < 1f; f += 1f / 12f)
        {
            Vector2 velocity = new Vector2(3f).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
            Dust dust = Dust.NewDustPerfect(target.Center, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.4f, 0.6f));
            dust.noGravity = true;
            dust.color = color.WithLuminance(0.1f);
            dust.alpha = 15;
        }
    }


    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        int length = ProjectileID.Sets.TrailCacheLength[Type];

        for (int i = 0; i < oldPosLerped.Length; i++)
        {
            if (i == length)
                continue;

            if (i % 12 == 0)
            {
                Vector2 position = oldPosLerped[i]; //+ Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Vector2 position2 = oldPosLerped[i + 1]; //+ Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                float nul = 0f;
                if (position != Vector2.Zero && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, position2, 10, ref nul))
                    return true;
            }
        }

        return base.Colliding(projHitbox, targetHitbox);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (AI_Timer < 40)
            return false;

        int length = ProjectileID.Sets.TrailCacheLength[Type];
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 origin = texture.Size() / 2f;

        for (int i = length; i > 0; i--)
        {
            if (i >= length)
                continue;

            Color color = Projectile.GetAlpha(lightColor) * Utility.QuadraticEaseInOut(Projectile.Opacity);
            float rotation = oldRotLerped[i] + MathHelper.PiOver2;
            SpriteEffects effects = ((Projectile.oldSpriteDirection[i] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            if (oldPosLerped[i] == Vector2.Zero)
                continue;

            Vector2 position = oldPosLerped[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            float scale = 0.4f * MathHelper.Clamp((1f - Math.Abs(i - length / 2f) / (length / 2f)), 0, 1);

            for (int j = 0; j < 5; j++)
                Main.EntitySpriteDraw(texture, position, null, color.WithAlpha(0), rotation, origin, scale, effects);
        }

        for (int i = length; i > 0; i--)
        {
            if (i >= length)
                continue;

            Color black = Color.Black.WithAlpha(255) * Projectile.Opacity;

            float rotation = oldRotLerped[i] + MathHelper.PiOver2;
            SpriteEffects effects = ((Projectile.oldSpriteDirection[i] == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            if (oldPosLerped[i] == Vector2.Zero)
                continue;

            Vector2 position = oldPosLerped[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            float scale = 0.4f * MathHelper.Clamp((1f - Math.Abs(i - length / 2f) / (length / 2f)), 0, 1);

            for (int j = 0; j < 3; j++)
                Main.EntitySpriteDraw(texture, position, null, black, rotation, origin, scale, effects);
        }

        for (int i = length; i > 0; i--)
        {
            if (i >= length)
                continue;

            if (i % 12 == 0)
            {
                Vector2 position = oldPosLerped[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                float scale = 2f * MathHelper.Clamp((1f - Math.Abs(i - length / 2f) / (length / 2f)), 0, 1) * Projectile.Opacity;
                Utility.DrawStar(position, 1, (color * Projectile.Opacity).WithAlpha(0), scale, oldRotLerped[i] + MathHelper.PiOver2);
            }
        }


        return false;
    }

    public override Color? GetAlpha(Color lightColor) => color.WithAlpha(200);
}
