using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class SeleniteSwordProjectile : ModProjectile
    {
        public override string Texture => Macrocosm.TexturesPath + "Swing";

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.scale = 1f;
        }

        private int spawnTimeLeft;
        private bool spawned;

        public override void AI()
        {
            if (!spawned)
            {
                spawnTimeLeft = Projectile.timeLeft;
                spawned = true;
            }

            Projectile.rotation -= 0.25f;
            Projectile.velocity *= 1.02f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Particle.Create<SeleniteStar>((p) =>
            {
                p.Position = target.Center;
                p.Velocity = -Vector2.UnitY * 0.4f;
                p.Scale = new(1.2f);
                p.Rotation = MathHelper.PiOver4;
            }, shouldSync: true
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, 4, frameY: 3);
            Vector2 origin = new(frame.Width / 2, frame.Height / 2);

            int count = 4;
            for (int i = 0; i < count; i++)
            {
                Vector2 position = Projectile.Center + Utility.PolarVector(1, Projectile.rotation + (MathHelper.TwoPi / count) * i);
                SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

                float progress = (float)Projectile.timeLeft / spawnTimeLeft;
                float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

                float rotation = (Projectile.Center - position).ToRotation() - MathHelper.Pi;
                Color color = new Color(130, 220, 199).WithOpacity(1f - progress);
                float scale = Projectile.scale + Projectile.scale * ((float)i / count);

                Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(1, 4, frameY: 0), color * progressScale, rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, scale * 0.95f, effects, 0f);
                Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(1, 4, frameY: 1), color * 0.15f, rotation, origin, scale, effects, 0f);
                Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(1, 4, frameY: 2), color * 0.7f * progressScale * 0.3f, rotation, origin, scale, effects, 0f);
                Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(1, 4, frameY: 2), color * 0.8f * progressScale * 0.5f, rotation, origin, scale * 0.975f, effects, 0f);
                Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(1, 4, frameY: 3), (Color.White * (0.2f - 0.2f * progressScale)).WithOpacity(0.4f - 0.4f * progressScale), rotation, origin, scale * 0.95f, effects, 0f);
                Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.2f - 0.2f * progressScale), rotation, origin, scale * 0.75f, effects, 0f);
                Main.EntitySpriteDraw(texture, position - Main.screenPosition, texture.Frame(1, 4, frameY: 3), (color * progressScale).WithOpacity(0.1f - 0.05f * progressScale), rotation, origin, scale * 0.55f, effects, 0f);

                /*
                int iteration = 0;
                for (float f = 0f; f < 16f; f += 0.04f)
                {
                    float starProgress = f / 16f;

                    float angle = rotation + Math.Sign(Projectile.velocity.X) * (f - 2f) * ((float)Math.PI * -2f) * 0.025f + MathHelper.Pi * 0.17f * Math.Sign(Projectile.velocity.X);
                    Vector2 drawpos = position + angle.ToRotationVector2() * ((float)frame.Height - 8f) * Projectile.scale * MathHelper.Lerp(1.025f, 1.005f, starProgress);
                    Utility.DrawSwingEffectStar(1f, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0), color, progress, 0f, 0.5f, 1f, 1f, angle, new Vector2(0.1f + 0.2f * starProgress, 1.1f) * (1f - starProgress) * progress, Vector2.One * 0.7f);

                    if (iteration++ == 0)
                        Utility.DrawSwingEffectStar(1f, SpriteEffects.None, drawpos + new Vector2(10, 0).RotatedBy(rotation) * progress, new Color(255, 255, 255, 0), color, progress, 0f, 0.5f, 1f, 1f, angle, new Vector2(0.2f, 1.2f) * (1f - starProgress), new Vector2(10f, 20f));
                }
                */
            }

            return false;
        }
    }
}