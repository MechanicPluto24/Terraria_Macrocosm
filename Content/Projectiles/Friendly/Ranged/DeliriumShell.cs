using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class DeliriumShell : ModProjectile, IBullet, IExplosive
    {
        public float BlastRadius => 100;

        public int OriginalWidth => 14;
        public int OriginalHeight => 14;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(14);
            AIType = ProjectileID.Bullet;
            Projectile.width = OriginalWidth;
            Projectile.height = OriginalHeight;
            Projectile.timeLeft = 270;
            Projectile.light = 0f;
        }

        bool spawned = false;
        float auraAlpha = 0f;
        public override bool PreAI()
        {
            Lighting.AddLight(Projectile.Center, new Color(101, 242, 139).ToVector3());

            if (auraAlpha < 1f)
                auraAlpha += 0.06f;

            if (!spawned && auraAlpha > 0.1f)
            {
                // spawn some dusts as "muzzle flash"
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PortalLightGreenDust>(), Scale: 2.2f);
                    dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(4f, 8f)).RotatedByRandom(MathHelper.PiOver4 * 0.6f) + Main.player[Projectile.owner].velocity;
                    dust.noLight = false;
                    dust.alpha = 200;
                    dust.noGravity = true;
                }
                spawned = true;
            }

            // spawn dust trail 
            if (Main.rand.NextBool(1))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PortalLightGreenDust>(), Scale: 1.3f);
                dust.noLight = false;
                dust.alpha = 255;
                dust.noGravity = true;
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            //spawn dust explosion on kill
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<PortalLightGreenDust>(), Scale: 1.6f);
                dust.velocity = (Vector2.UnitX * Main.rand.NextFloat(1f, 5f)).RotatedByRandom(MathHelper.TwoPi);
                dust.noLight = false;
                dust.noGravity = true;
            }

            var explosion = Particle.CreateParticle<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center + Projectile.oldVelocity;
                p.DrawColor = new Color(96, 237, 134) * 0.6f;
                p.Scale = 1.2f;
                p.NumberOfInnerReplicas = 6;
                p.ReplicaScalingFactor = 0.5f;
            });
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft < 3)
                return false;

            Texture2D tex = ModContent.Request<Texture2D>(Texture + "Aura").Value;

            var spriteBatch = Main.spriteBatch;
            state.SaveState(Main.spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            Main.EntitySpriteDraw(tex, Projectile.Center - new Vector2(0, 38).RotatedBy(Projectile.rotation - MathHelper.Pi) - Main.screenPosition, null, Color.White.WithOpacity(0.3f * auraAlpha), Projectile.rotation - MathHelper.Pi, tex.Size() / 2, 0.8f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(tex, Projectile.Center - new Vector2(0, 58).RotatedBy(Projectile.rotation - MathHelper.Pi) - Main.screenPosition, null, Color.White.WithOpacity(0.1f * auraAlpha), Projectile.rotation - MathHelper.Pi, tex.Size() / 2, 0.8f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(BlendState.AlphaBlend, state);

            Main.EntitySpriteDraw(tex, Projectile.Center - new Vector2(0, 18).RotatedBy(Projectile.rotation - MathHelper.Pi) - Main.screenPosition, null, Color.White.WithOpacity(0.6f), Projectile.rotation - MathHelper.Pi, tex.Size() / 2, 0.8f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(state);
            return true;
        }
    }
}
