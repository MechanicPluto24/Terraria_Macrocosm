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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteSwordSlash : ModProjectile
    {
        public override string Texture => Macrocosm.TexturesPath + "Swing";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public ref float Timer => ref Projectile.ai[0];
        public ref float Speed => ref Projectile.ai[1];

        private ArtemiteTrail trail;

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.Opacity = 0f;
            Projectile.timeLeft=500;
            trail = new();
        }

        /*
        public ref float SwingDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];
        public ref float Scale => ref Projectile.ai[2];
        */

        public override void AI()
        {
            Projectile.direction = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 0.2f * Projectile.direction;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * Speed;

            bool speedUp = Timer >= 40;

            if (Projectile.Opacity < 1f)
                Projectile.Opacity += 0.01f;

            trail.WidthMult = Utility.BackEaseInOut(Projectile.Opacity);

            for (int i = 0; i < (int)(8f * Projectile.Opacity); i++)
            {
                if (i % 3 == 0 && speedUp)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(35, 75).RotatedBy(Projectile.rotation), ModContent.DustType<ArtemiteBrightDust>(), default, Scale: Main.rand.NextFloat(0.6f, 1f)); ;
                    dust.velocity = Projectile.velocity ;
                    dust.color = Color.White.WithAlpha((byte)Main.rand.Next(255)) * Main.rand.NextFloat();
                    dust.noGravity = true;
                }

                if (speedUp || i % 3 == 0)
                    Particle.Create<ArtemiteStar>((p) =>
                    {
                        p.Position = Projectile.Center + Main.rand.NextVector2Circular(80, 80);
                        p.Velocity = speedUp ? Projectile.velocity * Main.rand.NextFloat(0.4f, 0.9f) : Vector2.Zero;
                        p.StarPointCount = speedUp ? 1 : 2;
                        p.Scale = speedUp ? new Vector2(Main.rand.NextFloat(1f, 2f)) : new Vector2(Main.rand.NextFloat(0.5f, 1.5f));
                        p.Rotation = speedUp ? Projectile.velocity.ToRotation() + MathHelper.PiOver2 : 0f;
                        p.FadeInNormalizedTime = 0.1f;
                        p.FadeOutNormalizedTime = 0.7f;
                    });
            }

            if (!speedUp)
                Speed *= 0.95f;

            if (speedUp && Speed < 40f)
                Speed *= 1.2f;

            Timer++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return null;
        }

        public override void CutTiles()
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle.Create<ArtemiteStar>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = -Vector2.UnitY * 0.4f;
                p.Scale = new(1f);
                p.Rotation = MathHelper.PiOver4;
            }, shouldSync: true
            );
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Particle.Create<ArtemiteStar>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = -Vector2.UnitY * 0.4f;
                p.Scale = new(1f);
                p.Rotation = MathHelper.PiOver4;
            }, shouldSync: true
            );
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            trail.Opacity = Projectile.Opacity;
            trail?.Draw(Projectile, Projectile.Size * 0.6f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Vector2 positionOffset = (Projectile.velocity.SafeNormalize(Vector2.UnitY)) * -100f;
            Vector2 position = (Projectile.Center + positionOffset) - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Frame(1, 4).Size() / 2f;
            float scale = Projectile.scale * 1.2f;
            SpriteEffects spriteEffects = ((!(Projectile.velocity.X >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None); // Flip the sprite based on the direction it is facing.
            float progress = 0.5f;
            float lightingColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            lightingColor = Utils.Remap(lightingColor, 0.2f, 1f, 0f, 1f);
            float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

            Color color = new Color(130, 220, 199) * 1.4f * Projectile.Opacity;
            Color middleMediumColor = color;

            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), middleMediumColor * 1f, Projectile.rotation + Projectile.spriteDirection * MathHelper.PiOver4 * -0.5f * (1f - progress), origin, scale * 1.05f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), middleMediumColor * 0.6f, Projectile.rotation + Projectile.spriteDirection * MathHelper.PiOver4 * -1f * (1f - progress), origin, scale * 1.05f, spriteEffects, 0f);
            Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), middleMediumColor * 0.2f, Projectile.rotation + Projectile.spriteDirection * MathHelper.PiOver4 * -1.5f * (1f - progress), origin, scale * 1.05f, spriteEffects, 0f);

            // This draws some sparkles around the circumference of the swing.
            for (float i = 0f; i < 14f; i += 1f)
            {
                float edgeRotation = Projectile.rotation + Projectile.spriteDirection * i * (MathHelper.Pi * -2f) * 0.02f + Utils.Remap(progress, 0f, 1f, 0f, MathHelper.PiOver4 * 1.6f) * Projectile.spriteDirection;
                Vector2 drawPos = position + edgeRotation.ToRotationVector2() * ((float)texture.Width * 0.5f - 1f) * scale;
                Utility.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, color.WithOpacity(Projectile.Opacity) * (i / 14f), middleMediumColor, progress, 0f, 0.5f, 0.5f, 1f, edgeRotation, new Vector2(0f, Utils.Remap(progress, 0f, 1f, 2f, 0f)) * scale, Vector2.One * scale);
            }

            return false;
        }
    }
}