using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class CrescentScriptureSlash : ModProjectile
    {
        public override string Texture => Macrocosm.TexturesPath + "Swing";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }

        public ref float Speed => ref Projectile.ai[0];
        private float progress;
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 120;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.Opacity = 0f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            progress = 0.5f;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.spriteDirection = Projectile.direction;

            if (Timer < 10 && Projectile.Opacity < 1f)
                Projectile.Opacity += 0.1f;

            if (Timer > 40)
                Projectile.Opacity -= 0.03f;

            if (Projectile.Opacity > 0f)
                Projectile.Opacity -= 0.004f;

            Speed *= 0.95f;

            if (Projectile.Opacity <= 0.01f && Timer > 10)
                Projectile.Kill();

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Speed;

            Lighting.AddLight(Projectile.Center, new Color(94, 229, 163).ToVector3());

            if (Speed > 4f)
            {
                for (int i = 0; i < 2; i++)
                    Particle.Create<PrettySparkle>((p) =>
                    {
                        Vector2 offset = Main.rand.NextVector2Circular(20, 100).RotatedBy(Projectile.rotation);
                        p.Position = Projectile.Center + offset;
                        p.Velocity = Projectile.velocity.SafeNormalize(default) * Main.rand.NextFloat();
                        p.DrawHorizontalAxis = true;
                        p.DrawVerticalAxis = false;
                        p.AdditiveAmount = 0.4f;
                        p.Scale = new Vector2(2f, Main.rand.NextFloat(0.5f, 1.5f)) * (1f - (Vector2.DistanceSquared(Projectile.Center, Projectile.Center + offset) / (100f * 100f)));
                        p.ScaleVelocity = new Vector2(Main.rand.NextFloat(0.01f, 0.02f));
                        p.Color = (Main.rand.NextBool() ? new Color(94, 229, 163, 255) : new Color(164, 101, 124, 255)) * Projectile.Opacity;
                        p.Rotation = Projectile.rotation;
                        p.TimeToLive = 40;
                        p.FadeInNormalizedTime = 0f;
                        p.FadeOutNormalizedTime = 0.2f;
                    });
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            for(int i = 45; i >= 0; i--)
            {
                float trailProgress = i == 0 ? 1f : MathHelper.Clamp(1f - (i / 45f), 0f, 1f) * 0.08f;
                Vector2 positionOffset = (Projectile.velocity.SafeNormalize(Vector2.UnitY)) * -100f ;
                Vector2 position = (Projectile.Center + positionOffset) - new Vector2(i * 2, 0).RotatedBy(Projectile.velocity.ToRotation()) - Main.screenPosition;
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Vector2 origin = texture.Frame(1, 4).Size() / 2f;
                float scale = Projectile.scale * 1.2f + i * 0.003f;
                if (i % 2 == 1) scale *= 1.1f;
                SpriteEffects spriteEffects = ((!(Projectile.velocity.X >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.

                Color color = new(94, 229, 163, 230);
                Color backDarkColor = color * 0.8f;
                Color middleMediumColor = color ;
                Color frontLightColor = new(146, 246, 216, 230);
                Color secondaryColor = new(240, 146, 180, 230);

                if (i > 1)
                {
                    color = color.WithOpacity(0.4f);
                    frontLightColor = frontLightColor.WithOpacity(0.4f);
                    secondaryColor = secondaryColor.WithOpacity(0.4f);
                }

                if (i % 2 == 1) trailProgress *= 0.5f;

                color *= trailProgress * Projectile.Opacity;
                backDarkColor *= trailProgress * Projectile.Opacity;
                middleMediumColor *= trailProgress * Projectile.Opacity;
                frontLightColor *= trailProgress * Projectile.Opacity;
                secondaryColor *= trailProgress * Projectile.Opacity;

                float rotation = Projectile.rotation + MathHelper.Pi / 6 * Projectile.direction;
                Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), secondaryColor, rotation + Projectile.spriteDirection * MathHelper.PiOver4 * -3f * (1f - progress), origin, scale * 1.05f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), backDarkColor, rotation + Projectile.spriteDirection * MathHelper.PiOver4 * -2f * (1f - progress), origin, scale * 1.05f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), middleMediumColor, rotation + Projectile.spriteDirection * MathHelper.PiOver4 * -1f * (1f - progress), origin, scale * 1.05f, spriteEffects, 0f);
                Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), frontLightColor, rotation + Projectile.spriteDirection * MathHelper.PiOver4 * 0f * (1f - progress), origin, scale * 1.05f, spriteEffects, 0f);

                // This draws some sparkles around the circumference of the swing.
                if(i == 0)
                {
                    for (float f = 0f; f < 14f; f += 1f)
                {
                    float edgeRotation = rotation + Projectile.spriteDirection * f * (MathHelper.Pi * -2f) * 0.03f + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4 * 1.6f) * Projectile.spriteDirection;
                    Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 1f) * scale;
                    Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, color.WithOpacity(Projectile.Opacity) * (f / 14f) * trailProgress, color, progress, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 2f, 0f)) * scale, Vector2.One * scale);
                }
                }
            }
       
            return false;
        }


    }
}